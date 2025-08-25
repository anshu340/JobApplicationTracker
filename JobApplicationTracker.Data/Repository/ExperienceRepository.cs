using Dapper;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using System.Data;

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
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = @"
                    SELECT ExperienceId, IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, 
                           Description, JobTitle, Organization, Location
                    FROM Experiences
                    ORDER BY StartYear DESC, StartMonth DESC";

                return await connection.QueryAsync<ExperienceDto>(sql).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving experiences: {ex.Message}", ex);
            }
        }

        // Get experience by Id
        public async Task<ExperienceDto?> GetExperienceByIdAsync(int experienceId)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving experience by ID: {ex.Message}", ex);
            }
        }// Get experiences for a user using JSON array in Users table
        public async Task<IEnumerable<ExperienceDataModel>> GetExperiencesByUserIdAsync(int userId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                // 1. Get JSON array string of Experience IDs from Users table
                var query = "SELECT Experiences FROM Users WHERE UserId = @UserId";
                var experienceIds = await connection.QueryFirstOrDefaultAsync<string>(query, new { UserId = userId });

                if (string.IsNullOrWhiteSpace(experienceIds))
                    return Enumerable.Empty<ExperienceDataModel>();

                // 2. Parse JSON array - remove brackets and split by comma
                var cleanedIds = experienceIds.Trim('[', ']');
                if (string.IsNullOrWhiteSpace(cleanedIds))
                    return Enumerable.Empty<ExperienceDataModel>();

                var ids = cleanedIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                   .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                                   .Where(id => id > 0)
                                   .ToList();

                if (!ids.Any())
                    return Enumerable.Empty<ExperienceDataModel>();

                // 3. Get Experiences from Experiences table
                var experiencesQuery = $@"
            SELECT ExperienceId, IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, 
                   Description, JobTitle, Organization, Location
            FROM Experiences
            WHERE ExperienceId IN ({string.Join(",", ids)})
            ORDER BY StartYear DESC, StartMonth DESC";

                return await connection.QueryAsync<ExperienceDataModel>(experiencesQuery);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user experiences: {ex.Message}", ex);
            }
        }
        // Single method that handles both INSERT and UPDATE
        public async Task<ResponseDto> SubmitExperienceAsync(ExperienceDataModel experience)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                if (experience.ExperienceId > 0)
                {
                    // UPDATE existing experience
                    var updateSql = @"
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

                    var parameters = CreateParameters(experience);
                    parameters.Add("@ExperienceId", experience.ExperienceId);

                    var affectedRows = await connection.ExecuteAsync(updateSql, parameters).ConfigureAwait(false);

                    return new ResponseDto
                    {
                        IsSuccess = affectedRows > 0,
                        Message = affectedRows > 0 ? "Experience updated successfully." : "Experience not found.",
                        Id = experience.ExperienceId
                    };
                }
                else
                {
                    // INSERT new experience
                    var insertSql = @"
                        INSERT INTO Experiences 
                            (IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, 
                             Description, JobTitle, Organization, Location)
                        VALUES 
                            (@IsCurrentlyWorking, @StartMonth, @StartYear, @EndMonth, @EndYear, 
                             @Description, @JobTitle, @Organization, @Location);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";

                    var parameters = CreateParameters(experience);
                    var newId = await connection.QuerySingleAsync<int>(insertSql, parameters).ConfigureAwait(false);

                    return new ResponseDto
                    {
                        IsSuccess = newId > 0,
                        Message = newId > 0 ? "Experience created successfully." : "Failed to create experience.",
                        Id = newId
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error submitting experience: {ex.Message}",
                    Id = experience.ExperienceId
                };
            }
        }



        // Delete experience
        public async Task<ResponseDto> DeleteExperienceAsync(int experienceId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    // 1. Remove experience from all users first
                    var usersWithExperience = await connection.QueryAsync<(int UserId, string Experiences)>(
                        "SELECT UserId, Experiences FROM Users WHERE Experiences IS NOT NULL AND Experiences LIKE @Pattern",
                        new { Pattern = $"%{experienceId}%" }, transaction);

                    foreach (var (userId, experiences) in usersWithExperience)
                    {
                        var experienceIdsList = experiences.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                          .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                                                          .Where(id => id > 0 && id != experienceId)
                                                          .ToList();

                        var newExperienceIds = experienceIdsList.Any() ? string.Join(",", experienceIdsList) : null;

                        await connection.ExecuteAsync(
                            "UPDATE Users SET Experiences = @Experiences, UpdatedAt = @UpdatedAt WHERE UserId = @UserId",
                            new { Experiences = newExperienceIds, UpdatedAt = DateTime.UtcNow, UserId = userId }, transaction);
                    }

                    // 2. Delete the experience
                    var sql = "DELETE FROM Experiences WHERE ExperienceId = @ExperienceId";
                    var affectedRows = await connection.ExecuteAsync(sql, new { ExperienceId = experienceId }, transaction);

                    transaction.Commit();

                    return new ResponseDto
                    {
                        IsSuccess = affectedRows > 0,
                        Message = affectedRows > 0
                            ? "Experience deleted successfully."
                            : "Experience not found.",
                        Id = experienceId
                    };
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error deleting experience: {ex.Message}",
                    Id = experienceId
                };
            }
        }

        // Helper method to create common parameters
        private DynamicParameters CreateParameters(ExperienceDataModel experience)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@IsCurrentlyWorking", experience.IsCurrentlyWorking, DbType.Boolean);
            parameters.Add("@StartMonth", experience.StartMonth, DbType.Int32);
            parameters.Add("@StartYear", experience.StartYear, DbType.Int32);
            parameters.Add("@EndMonth", experience.EndMonth, DbType.Int32);
            parameters.Add("@EndYear", experience.EndYear, DbType.Int32);
            parameters.Add("@Description", experience.Description ?? string.Empty, DbType.String);
            parameters.Add("@JobTitle", experience.JobTitle ?? string.Empty, DbType.String);
            parameters.Add("@Organization", experience.Organization ?? string.Empty, DbType.String);
            parameters.Add("@Location", experience.Location ?? string.Empty, DbType.String);

            return parameters;
        }
    }
}
