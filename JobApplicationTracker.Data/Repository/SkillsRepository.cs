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
            var sql = @"SELECT SkillId, SkillName, Category FROM [dbo].[Skill]";
            return await connection.QueryAsync<SkillsDataModel>(sql).ConfigureAwait(false);
        }

        public async Task<SkillsDataModel> GetSkillsByIdAsync(int skillId)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();
            var sql = """
                      SELECT SkillId, 
                             SkillName, 
                             Category 
                      FROM [dbo].[Skill]
                      WHERE SkillId = @SkillId
                      """;
            var parameters = new DynamicParameters();
            parameters.Add("@SkillId", skillId, DbType.Int32);
            return await connection.QueryFirstOrDefaultAsync<SkillsDataModel>(sql, parameters).ConfigureAwait(false);
        }

        public async Task<ResponseDto> SubmitSkillsAsync(SkillsDataModel skillsDto)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            // Check if SkillId exists
            var existsQuery = "SELECT COUNT(1) FROM [dbo].[Skill] WHERE SkillId = @SkillId";
            var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new { skillsDto.SkillId });

            int skillIdResult = 0;
            int affectedRows = 0;

            if (skillsDto.SkillId <= 0 || exists == 0)
            {
                // Insert new skill and get inserted SkillId
                var sql = @"
            INSERT INTO [dbo].[Skill] (SkillName, Category)
            OUTPUT INSERTED.SkillId
            VALUES (@SkillName, @Category)";

                skillIdResult = await connection.ExecuteScalarAsync<int>(sql, new
                {
                    skillsDto.SkillName,
                    skillsDto.Category
                });

                affectedRows = skillIdResult > 0 ? 1 : 0;
            }
            else
            {
                // Update existing skill
                var sql = @"
            UPDATE [dbo].[Skill]
            SET SkillName = @SkillName,
                Category = @Category
            WHERE SkillId = @SkillId";

                affectedRows = await connection.ExecuteAsync(sql, new
                {
                    skillsDto.SkillName,
                    skillsDto.Category,
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
            var sql = @"DELETE FROM [dbo].[Skill] WHERE SkillId = @SkillId";

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









