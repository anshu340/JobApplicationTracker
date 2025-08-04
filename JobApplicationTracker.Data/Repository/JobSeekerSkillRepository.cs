using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

public class JobSeekerSkillsRepository : IJobSeekerSkillRepository
{
    private readonly IDatabaseConnectionService _connectionService;

    public JobSeekerSkillsRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<IEnumerable<JobSeekerSkill>> GetAllJobSeekerSkillsAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT JobSeekerSkillId, 
                         JobSeekerId,
                         SkillId,
                         ProficiencyLevel
                  FROM JobSeekerSkill
                  """;

        return await connection.QueryAsync<JobSeekerSkill>(sql).ConfigureAwait(false);
    }

    public async Task<JobSeekerSkill> GetJobSeekerSkillsByIdAsync(int jobSeekerSkillId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT JobSeekerSkillId, 
                         JobSeekerId,
                         SkillId,
                         ProficiencyLevel
                  FROM JobSeekerSkill
                  WHERE JobSeekerSkillId = @JobSeekerSkillId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerSkillId", jobSeekerSkillId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobSeekerSkill>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<IEnumerable<JobSeekerSkill>> GetJobSeekerSkillsByJobSeekerIdAsync(int jobSeekerId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT JobSeekerSkillId, 
                         JobSeekerId,
                         SkillId,
                         ProficiencyLevel
                  FROM JobSeekerSkill
                  WHERE JobSeekerId = @JobSeekerId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerId", jobSeekerId, DbType.Int32);

        return await connection.QueryAsync<JobSeekerSkill>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitJobSeekerSkillsAsync(JobSeekerSkill jobSeekerSkillsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (jobSeekerSkillsDto.JobSeekerSkillId <= 0)
        {
            // Insert new job seeker skill (JobSeekerSkillId is auto-incremented)
            sql = """
                  INSERT INTO JobSeekerSkill (JobSeekerId, SkillId, ProficiencyLevel)
                  VALUES (@JobSeekerId, @SkillId, @ProficiencyLevel);
                  SELECT CAST(SCOPE_IDENTITY() AS INT);
                  """;
        }
        else
        {
            // Update existing job seeker skill
            sql = """
                  UPDATE JobSeekerSkill
                  SET 
                      JobSeekerId = @JobSeekerId,
                      SkillId = @SkillId,
                      ProficiencyLevel = @ProficiencyLevel
                  WHERE JobSeekerSkillId = @JobSeekerSkillId
                  """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerSkillId", jobSeekerSkillsDto.JobSeekerSkillId, DbType.Int32);
        parameters.Add("@JobSeekerId", jobSeekerSkillsDto.JobSeekerId, DbType.Int32);
        parameters.Add("@SkillId", jobSeekerSkillsDto.SkillId, DbType.Int32);
        parameters.Add("@ProficiencyLevel", jobSeekerSkillsDto.ProficiencyLevel, DbType.Int32);

        var affectedRows = 0;

        if (jobSeekerSkillsDto.JobSeekerSkillId <= 0)
        {
            // Insert operation
            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            jobSeekerSkillsDto.JobSeekerSkillId = newId; // Set the ID for the newly inserted record
        }
        else
        {
            // Update operation
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Job seeker skill submitted successfully." : "Failed to submit job seeker skill."
        };
    }

    public async Task<ResponseDto> DeleteJobSeekerSkillsAsync(int jobSeekerSkillId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a job seeker skill by ID
        var sql = """DELETE FROM JobSeekerSkill WHERE JobSeekerSkillId = @JobSeekerSkillId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerSkillId", jobSeekerSkillId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete job seeker skill."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Job seeker skill deleted successfully."
        };
    }

    public async Task<ResponseDto> DeleteJobSeekerSkillsByJobSeekerIdAsync(int jobSeekerId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """DELETE FROM JobSeekerSkill WHERE JobSeekerId = @JobSeekerId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerId", jobSeekerId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = true,
            Message = affectedRows > 0 ? $"Deleted {affectedRows} job seeker skill(s) successfully." : "No job seeker skills found to delete."
        };
    }
}