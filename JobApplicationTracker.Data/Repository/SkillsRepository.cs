using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

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

        var sql = @"SELECT SkillId, SkillName, Category FROM Skills";

        return await connection.QueryAsync<SkillsDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<SkillsDataModel> GetSkillsByIdAsync(int skillId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        // write the SQL query to fetch a skill by ID
        var sql = """
                  SELECT SkillId, 
                         SkillName, 
                         Category 
                  FROM Skills
                  WHERE SkillId = @SkillId
                  """;
        

        var parameters = new DynamicParameters();
        parameters.Add("@SkillId", skillId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<SkillsDataModel>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitSkillsAsync(SkillsDataModel skillsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (skillsDto.SkillId <= 0)
        {
            // Insert new skill (assumes JobTypeId is auto-incremented)
            sql = """
            INSERT INTO Skills (SkillName, Category)
            VALUES (@SkillName, @Category)
            """;
        }
        else
        {
            // Update existing skill
            sql = """
            UPDATE Skills
            SET SkillName = SkillName,
                Category = Category
            WHERE SkillId = SkillId
            """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("SkillId", skillsDto.SkillId, DbType.Int32);
        parameters.Add("SkillName", skillsDto.SkillName, DbType.String);
        parameters.Add("Category", skillsDto.Category, DbType.String);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Jobs type submitted successfully." : "Failed to submit job type."
        };
    }



    public async Task<ResponseDto> DeleteSkillsAsync(int skillId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        // write the SQL query to delete askill by ID
        var sql = """DELETE FROM Skills WHERE SkillId = SkillId""";

        var parameters = new DynamicParameters();
        parameters.Add("@SkillId", skillId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete Skill."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Skill deleted successfully."
        };
    }
}
