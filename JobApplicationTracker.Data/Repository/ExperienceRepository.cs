using Dapper;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;

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
        public async Task<IEnumerable<ExperienceDataModel>> GetAllExperiencesAsync()
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = @"
                SELECT ExperienceId, IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, Description
                FROM Experiences";

            return await connection.QueryAsync<ExperienceDataModel>(sql).ConfigureAwait(false);
        }

        // Get experience by Id
        public async Task<ExperienceDataModel> GetExperienceByIdAsync(int experienceId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = @"
                SELECT ExperienceId, IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, Description
                FROM Experiences
                WHERE ExperienceId = @ExperienceId";

            return await connection.QueryFirstOrDefaultAsync<ExperienceDataModel>(
                sql, new { ExperienceId = experienceId }).ConfigureAwait(false);
        }

        // Insert new experience
        public async Task<ResponseDto> SubmitExperienceAsync(ExperienceDataModel experience)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = @"
                INSERT INTO Experiences 
                    (IsCurrentlyWorking, StartMonth, StartYear, EndMonth, EndYear, Description)
                VALUES 
                    (@IsCurrentlyWorking, @StartMonth, @StartYear, @EndMonth, @EndYear, @Description);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new DynamicParameters();
            parameters.Add("@IsCurrentlyWorking", experience.IsCurrentlyWorking, DbType.Boolean);
            parameters.Add("@StartMonth", experience.StartMonth, DbType.Int32);
            parameters.Add("@StartYear", experience.StartYear, DbType.Int32);
            parameters.Add("@EndMonth", experience.EndMonth, DbType.Int32);
            parameters.Add("@EndYear", experience.EndYear, DbType.Int32);
            parameters.Add("@Description", experience.Description, DbType.String);

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
                    Description = @Description
                WHERE ExperienceId = @ExperienceId";

            var parameters = new DynamicParameters();
            parameters.Add("@ExperienceId", experienceId);
            parameters.Add("@IsCurrentlyWorking", experience.IsCurrentlyWorking, DbType.Boolean);
            parameters.Add("@StartMonth", experience.StartMonth, DbType.Int32);
            parameters.Add("@StartYear", experience.StartYear, DbType.Int32);
            parameters.Add("@EndMonth", experience.EndMonth, DbType.Int32);
            parameters.Add("@EndYear", experience.EndYear, DbType.Int32);
            parameters.Add("@Description", experience.Description, DbType.String);

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
