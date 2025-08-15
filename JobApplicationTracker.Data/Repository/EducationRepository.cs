using Dapper;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using System.Data;

namespace JobApplicationTracker.Data.Repository
{
    public class EducationRepository(IDatabaseConnectionService connectionService) : IEducationRepository
    {
        public async Task<IEnumerable<EducationDto>> GetAllEducationAsync()
        {
            await using var connection = await connectionService.GetDatabaseConnectionAsync();
            var sql = """
                SELECT EducationId, School, Degree, FieldOfStudy, StartDate, EndDate,
                       IsCurrentlyStudying, Description, GPA
                FROM Education
            """;
            return await connection.QueryAsync<EducationDto>(sql);
        }

        public async Task<EducationDto?> GetEducationByIdAsync(int educationId)
        {
            await using var connection = await connectionService.GetDatabaseConnectionAsync();
            var sql = """
                SELECT EducationId, School, Degree, FieldOfStudy, StartDate, EndDate,
                       IsCurrentlyStudying, Description, GPA
                FROM Education
                WHERE EducationId = @EducationId
            """;
            return await connection.QueryFirstOrDefaultAsync<EducationDto>(sql, new { EducationId = educationId });
        }

        public async Task<ResponseDto> SubmitEducationAsync(EducationDto dto)
        {
            await using var connection = await connectionService.GetDatabaseConnectionAsync();
            var isNew = dto.EducationId is null or <= 0;

            if (isNew)
            {
                var insertQuery = @"
                    INSERT INTO Education (School, Degree, FieldOfStudy, StartDate, EndDate,
                                            IsCurrentlyStudying, Description, GPA)
                    VALUES (@School, @Degree, @FieldOfStudy, @StartDate, @EndDate,
                            @IsCurrentlyStudying, @Description, @GPA);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                ";

                var newId = await connection.ExecuteScalarAsync<int>(insertQuery, dto);
                return new ResponseDto
                {
                    IsSuccess = newId > 0,
                    Message = "Education added successfully.",
                    Id = newId
                };
            }
            else
            {
                var updateQuery = @"
                    UPDATE Education
                    SET School = @School,
                        Degree = @Degree,
                        FieldOfStudy = @FieldOfStudy,
                        StartDate = @StartDate,
                        EndDate = @EndDate,
                        IsCurrentlyStudying = @IsCurrentlyStudying,
                        Description = @Description,
                        GPA = @GPA
                    WHERE EducationId = @EducationId
                ";

                var rows = await connection.ExecuteAsync(updateQuery, dto);
                return new ResponseDto
                {
                    IsSuccess = rows > 0,
                    Message = rows > 0 ? "Education updated successfully." : "Failed to update education.",
                    Id = dto.EducationId ?? -1
                };
            }
        }
        // New method to get Education details by UserId (Education JSON array in Users table)
        public async Task<IEnumerable<EducationDto>> GetEducationByUserIdAsync(int userId)
        {
            await using var connection = await connectionService.GetDatabaseConnectionAsync();

            // 1. Get Education JSON array string from Users table
            var userEducationQuery = """
        SELECT Education 
        FROM [dbo].[Users] 
        WHERE UserId = @UserId
    """;

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int32);

            var educationJson = await connection.QueryFirstOrDefaultAsync<string>(userEducationQuery, parameters);

            if (string.IsNullOrWhiteSpace(educationJson))
            {
                return Enumerable.Empty<EducationDto>();
            }

            try
            {
                // 2. Deserialize JSON array of education IDs
                var educationIds = System.Text.Json.JsonSerializer.Deserialize<int[]>(educationJson);

                if (educationIds == null || educationIds.Length == 0)
                    return Enumerable.Empty<EducationDto>();

                // 3. Query Education table for these IDs
                var educationQuery = """
            SELECT EducationId, School, Degree, FieldOfStudy, StartDate, EndDate,
                   IsCurrentlyStudying, Description, GPA
            FROM Education
            WHERE EducationId IN @EducationIds
        """;

                var educationParams = new DynamicParameters();
                educationParams.Add("@EducationIds", educationIds);

                return await connection.QueryAsync<EducationDto>(educationQuery, educationParams);
            }
            catch (System.Text.Json.JsonException)
            {
                // Handle invalid JSON
                return Enumerable.Empty<EducationDto>();
            }
        }

        // Method to get only Education IDs by UserId from Users table (JSON array format)
        public async Task<IEnumerable<int>> GetEducationIdsByUserIdAsync(int userId)
        {
            await using var connection = await connectionService.GetDatabaseConnectionAsync();

            var sql = """
        SELECT Education
        FROM [dbo].[Users]
        WHERE UserId = @UserId
    """;

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int32);

            var educationJson = await connection.QueryFirstOrDefaultAsync<string>(sql, parameters);

            if (string.IsNullOrWhiteSpace(educationJson))
            {
                return Enumerable.Empty<int>();
            }

            try
            {
                var educationIds = System.Text.Json.JsonSerializer.Deserialize<int[]>(educationJson);
                return educationIds ?? Enumerable.Empty<int>();
            }
            catch (System.Text.Json.JsonException)
            {
                return Enumerable.Empty<int>();
            }
        }


        public async Task<ResponseDto> DeleteEducationAsync(int educationId)
        {
            await using var connection = await connectionService.GetDatabaseConnectionAsync();
            var sql = "DELETE FROM Education WHERE EducationId = @EducationId";
            var rows = await connection.ExecuteAsync(sql, new { EducationId = educationId });

            return new ResponseDto
            {
                IsSuccess = rows > 0,
                Message = rows > 0 ? "Education deleted successfully." : "Failed to delete education."
            };
        }
    }
}
