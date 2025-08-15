using Dapper;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using System.Data;
using System.Text.Json;

namespace JobApplicationTracker.Data.Repository
{
    public class ExperienceRepository : IExperienceRepository
    {
        private readonly IDatabaseConnectionService _connectionService;

        public ExperienceRepository(IDatabaseConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        // Get all experiences
        public async Task<IEnumerable<ExperienceDto>> GetAllExperiencesAsync()
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = @"
                SELECT ExperienceId, IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, 
                       Description, JobTitle, Organization, Location
                FROM Experiences";

            return await connection.QueryAsync<ExperienceDto>(sql).ConfigureAwait(false);
        }

        // Get experience by Id
        public async Task<ExperienceDto?> GetExperienceByIdAsync(int experienceId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = @"
                SELECT ExperienceId, IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, 
                       Description, JobTitle, Organization, Location
                FROM Experiences
                WHERE ExperienceId = @ExperienceId";

            return await connection.QueryFirstOrDefaultAsync<ExperienceDto>(
                sql, new { ExperienceId = experienceId }).ConfigureAwait(false);
        }

        // Get experiences for a user using JSON array in Users table
        public async Task<IEnumerable<ExperienceDataModel>> GetExperiencesByUserIdAsync(int userId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            // 1. Get JSON array of Experience IDs from Users table
            var query = "SELECT Experiences FROM [dbo].[Users] WHERE UserId = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int32);

            var experienceJson = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);

            if (string.IsNullOrWhiteSpace(experienceJson))
                return Enumerable.Empty<ExperienceDataModel>();

            try
            {
                var experienceIds = JsonSerializer.Deserialize<int[]>(experienceJson);
                if (experienceIds == null || experienceIds.Length == 0)
                    return Enumerable.Empty<ExperienceDataModel>();

                // 2. Get Experiences from Experiences table
                var experiencesQuery = @"
                    SELECT ExperienceId, IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, 
                           Description, JobTitle, Organization, Location
                    FROM Experiences
                    WHERE ExperienceId IN @ExperienceIds";

                var experienceParams = new DynamicParameters();
                experienceParams.Add("@ExperienceIds", experienceIds);

                return await connection.QueryAsync<ExperienceDataModel>(experiencesQuery, experienceParams);
            }
            catch (JsonException)
            {
                return Enumerable.Empty<ExperienceDataModel>();
            }
        }

        // Insert new experience
        public async Task<ResponseDto> SubmitExperienceAsync(ExperienceDataModel experience)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = @"
                INSERT INTO Experiences 
                    (IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, 
                     Description, JobTitle, Organization, Location)
                VALUES 
                    (@IsCurrentlyWorking, @StartMonth, @StartYear, @EndMonth, @EndYear, 
                     @Description, @JobTitle, @Organization, @Location);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new DynamicParameters();
            parameters.Add("@IsCurrentlyWorking", experience.IsCurrentlyWorking, DbType.Boolean);
            parameters.Add("@StartMonth", experience.StartMonth, DbType.Int32);
            parameters.Add("@StartYear", experience.StartYear, DbType.Int32);
            parameters.Add("@EndMonth", experience.EndMonth, DbType.Int32);
            parameters.Add("@EndYear", experience.EndYear, DbType.Int32);
            parameters.Add("@Description", experience.Description, DbType.String);
            parameters.Add("@JobTitle", experience.JobTitle, DbType.String);
            parameters.Add("@Organization", experience.Organization, DbType.String);
            parameters.Add("@Location", experience.Location, DbType.String);

            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            experience.ExperienceId = newId;

            return new ResponseDto
            {
                IsSuccess = newId > 0,
                Message = newId > 0 ? "Experience created successfully." : "Failed to create experience.",
                Id = newId
            };
        }

        // Update existing experience
        public async Task<ResponseDto> UpdateExperienceAsync(int experienceId, ExperienceDataModel experience)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = @"
                UPDATE Experiences
                SET 
                    IsCurrentlyWorking = @IsCurrentlyWorking,
                    StartMonth = @StartMonth,
                    StartYear = @StartYear,
                    EndMonth = @EndMonth,
                    EndYear = @EndYear,
                    Description = @Description,
                    JobTitle = @JobTitle,
                    Organization = @Organization,
                    Location = @Location
                WHERE ExperienceId = @ExperienceId";

            var parameters = new DynamicParameters();
            parameters.Add("@ExperienceId", experienceId);
            parameters.Add("@IsCurrentlyWorking", experience.IsCurrentlyWorking, DbType.Boolean);
            parameters.Add("@StartMonth", experience.StartMonth, DbType.Int32);
            parameters.Add("@StartYear", experience.StartYear, DbType.Int32);
            parameters.Add("@EndMonth", experience.EndMonth, DbType.Int32);
            parameters.Add("@EndYear", experience.EndYear, DbType.Int32);
            parameters.Add("@Description", experience.Description, DbType.String);
            parameters.Add("@JobTitle", experience.JobTitle, DbType.String);
            parameters.Add("@Organization", experience.Organization, DbType.String);
            parameters.Add("@Location", experience.Location, DbType.String);

            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                Message = affectedRows > 0 ? "Experience updated successfully." : "Experience not found or could not be updated.",
                Id = experienceId
            };
        }

        // Delete experience
        public async Task<ResponseDto> DeleteExperienceAsync(int experienceId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = "DELETE FROM Experiences WHERE ExperienceId = @ExperienceId";
            var affectedRows = await connection.ExecuteAsync(sql, new { ExperienceId = experienceId }).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                Message = affectedRows > 0
                    ? "Experience deleted successfully."
                    : "Experience not found or could not be deleted.",
                Id = experienceId
            };
        }
    }
}