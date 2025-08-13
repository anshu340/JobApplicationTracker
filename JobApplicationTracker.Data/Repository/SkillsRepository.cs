using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository
{
    public class SkillsRepository : ISkillsRepository
    {
        private readonly IDatabaseConnectionService _connectionService;

        public SkillsRepository(IDatabaseConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<IEnumerable<SkillsDataModel>> GetAllSkillsAsync()
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();
            var sql = @"SELECT SkillId, Skill FROM [dbo].[Skills]";
            return await connection.QueryAsync<SkillsDataModel>(sql).ConfigureAwait(false);
        }

        public async Task<SkillsDataModel> GetSkillsByIdAsync(int skillId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();
            var sql = """
                      SELECT SkillId, 
                             Skill 
                      FROM [dbo].[Skills]
                      WHERE SkillId = @SkillId
                      """;
            var parameters = new DynamicParameters();
            parameters.Add("@SkillId", skillId, DbType.Int32);
            return await connection.QueryFirstOrDefaultAsync<SkillsDataModel>(sql, parameters).ConfigureAwait(false);
        }

        // New method to get skills by UserId from Users table (JSON array format)
        public async Task<IEnumerable<SkillsDataModel>> GetSkillsByUserIdAsync(int userId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            // First get the skills JSON array from Users table
            var userSkillsQuery = """
                      SELECT Skills 
                      FROM [dbo].[Users] 
                      WHERE UserId = @UserId
                      """;
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int32);

            var skillsJson = await connection.QueryFirstOrDefaultAsync<string>(userSkillsQuery, parameters).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(skillsJson))
            {
                return Enumerable.Empty<SkillsDataModel>();
            }

            try
            {
                // Parse JSON array [1,2,3] to get skill IDs
                var skillIds = System.Text.Json.JsonSerializer.Deserialize<int[]>(skillsJson);

                if (skillIds == null || skillIds.Length == 0)
                {
                    return Enumerable.Empty<SkillsDataModel>();
                }

                // Get skill details for these IDs
                var skillsQuery = """
                          SELECT SkillId, Skill 
                          FROM [dbo].[Skills] 
                          WHERE SkillId IN @SkillIds
                          """;
                var skillsParams = new DynamicParameters();
                skillsParams.Add("@SkillIds", skillIds);

                return await connection.QueryAsync<SkillsDataModel>(skillsQuery, skillsParams).ConfigureAwait(false);
            }
            catch (System.Text.Json.JsonException)
            {
                // Handle case where Skills column contains invalid JSON
                return Enumerable.Empty<SkillsDataModel>();
            }
        }

        // Method to get only SkillIds by UserId from Users table (JSON array format)
        public async Task<IEnumerable<int>> GetSkillsIdByUserIdAsync(int userId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();
            var sql = """
                      SELECT Skills 
                      FROM [dbo].[Users] 
                      WHERE UserId = @UserId
                      """;
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId, DbType.Int32);

            var skillsJson = await connection.QueryFirstOrDefaultAsync<string>(sql, parameters).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(skillsJson))
            {
                return Enumerable.Empty<int>();
            }

            try
            {
                // Parse JSON array [1,2,3] to get skill IDs
                var skillIds = System.Text.Json.JsonSerializer.Deserialize<int[]>(skillsJson);
                return skillIds ?? Enumerable.Empty<int>();
            }
            catch (System.Text.Json.JsonException)
            {
                // Handle case where Skills column contains invalid JSON
                return Enumerable.Empty<int>();
            }
        }

        public async Task<ResponseDto> SubmitSkillsAsync(SkillsDataModel skillsDto)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            // Check if SkillId exists
            var existsQuery = "SELECT COUNT(1) FROM [dbo].[Skills] WHERE SkillId = @SkillId";
            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new { skillsDto.SkillId });

            int skillIdResult = 0;
            int affectedRows = 0;

            if (skillsDto.SkillId <= 0 || exists == 0)
            {
                // Insert new skill and get inserted SkillId
                var sql = @"
            INSERT INTO [dbo].[Skills] (Skill)
            OUTPUT INSERTED.SkillId
            VALUES (@Skill)";

                skillIdResult = await connection.ExecuteScalarAsync<int>(sql, new
                {
                    skillsDto.Skill
                });

                affectedRows = skillIdResult > 0 ? 1 : 0;
            }
            else
            {
                // Update existing skill
                var sql = @"
            UPDATE [dbo].[Skills]
            SET Skill = @Skill
            WHERE SkillId = @SkillId";

                affectedRows = await connection.ExecuteAsync(sql, new
                {
                    skillsDto.Skill,
                    skillsDto.SkillId
                });

                skillIdResult = skillsDto.SkillId;
            }

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                StatusCode = affectedRows > 0 ? 0 : 1,
                Message = affectedRows > 0
                    ? "Skill submitted successfully."
                    : (skillsDto.SkillId > 0 ? $"Skill with ID {skillsDto.SkillId} not found for update." : "Failed to insert skill."),
                Id = skillIdResult
            };
        }

        public async Task<ResponseDto> DeleteSkillsAsync(int skillId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();
            var sql = @"DELETE FROM [dbo].[Skills] WHERE SkillId = @SkillId";

            var parameters = new DynamicParameters();
            parameters.Add("@SkillId", skillId, DbType.Int32);

            var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            if (affectedRows <= 0)
            {
                return new ResponseDto
                {
                    Id = skillId,
                    IsSuccess = false,
                    StatusCode = 1,
                    Message = $"Skill with ID {skillId} not found or could not be deleted."
                };
            }

            return new ResponseDto
            {
                Id = skillId,
                IsSuccess = true,
                StatusCode = 0,
                Message = "Skill deleted successfully."
            };
        }
    }

}


