using Dapper;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;

namespace JobApplicationTracker.Data.Repository;

public class JobRepository : IJobsRepository
{
    private readonly IDatabaseConnectionService _connectionService;

    public JobRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<IEnumerable<JobsDataModel>> GetAllJobsAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
            SELECT JobId,
                   CompanyId,
                   Title,
                   Description,
                   Requirements,
                   Location,
                   JobTypeId,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   ExperienceLevel,
                   Status,
                   PostedAt,
                   ApplicationDeadline
            FROM Job
            """;

        return await connection.QueryAsync<JobsDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<JobsDataModel> GetJobsByIdAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
            SELECT JobId,
                   CompanyId,
                   Title,
                   Description,
                   Requirements,
                   Location,
                   JobTypeId,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   ExperienceLevel,
                   Status,
                   ApplicationDeadline
            FROM Job
            WHERE JobId = @JobId
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobsDataModel>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitJobAsync(JobsDataModel jobsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();


        string sql;

        if (jobsDto.JobId <= 0)
        {
            // Insert new job
            sql = """
                INSERT INTO Job (
                        CompanyId,
                       Title,
                       Description,
                       Requirements,
                       Location,
                       JobTypeId,
                       SalaryRangeMin,
                       SalaryRangeMax,
                       ExperienceLevel,
                       Status,
                       PostedAt,
                       ApplicationDeadline
                )
                VALUES (
                    @PostedByUserId,
                @Title,
                @Description,
                @Requirements,
                @Location,
                @JobTypeId,
                @SalaryRangeMin,
                @SalaryRangeMax,
                @ExperienceLevel,
                @Status,
                @PostedAt,
    
                @ApplicationDeadline
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;
        }
        else
        {
            // Update existing job
            sql = """
                UPDATE Job
                SET
                    CompanyId = @PostedByUserId,
                    Title = @Title,
                    Description = @Description,
                    Requirements = @Requirements,
                    Location = @Location,
                    JobTypeId = @JobTypeId,
                    SalaryRangeMin = @SalaryRangeMin,
                    SalaryRangeMax = @SalaryRangeMax,
                    ExperienceLevel = @ExperienceLevel,
                    Status = @Status,
                    PostedAt = @PostedAt,
                    ApplicationDeadline = @ApplicationDeadline
                WHERE JobId = @JobId
                """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobsDto.JobId, DbType.Int32);
        parameters.Add("@PostedByUserId", jobsDto.CompanyId, DbType.Int32);
        parameters.Add("@Title", jobsDto.Title, DbType.String);
        parameters.Add("@Description", jobsDto.Description, DbType.String);
        parameters.Add("@Requirements", jobsDto.Requirements, DbType.String);
        parameters.Add("@Location", jobsDto.Location, DbType.String);
        parameters.Add("@JobTypeId", jobsDto.JobTypeId, DbType.Int32);
        parameters.Add("@SalaryRangeMin", jobsDto.SalaryRangeMin, DbType.Decimal);
        parameters.Add("@SalaryRangeMax", jobsDto.SalaryRangeMax, DbType.Decimal);
        parameters.Add("@ExperienceLevel", jobsDto.ExperienceLevel, DbType.Int32);
        parameters.Add("@Status", jobsDto.Status, DbType.Boolean);
        parameters.Add("@PostedAt", jobsDto.PostedAt, DbType.DateTime);
        parameters.Add("@ApplicationDeadline", jobsDto.ApplicationDeadline, DbType.DateTime);

        int affectedRows;

        if (jobsDto.JobId <= 0)
        {
            var newId = await connection.ExecuteScalarAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            jobsDto.JobId = newId;
        }
        else
        {
            // Ensure JobId is present and > 0
            if (jobsDto.PostedByUserId <= 0)
                return new ResponseDto { IsSuccess = false, Message = "Invalid JobId for update." };

            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }


        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Job submitted successfully." : "Failed to submit job."
        };
    }

    public async Task<ResponseDto> DeleteJobAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """DELETE FROM Job  WHERE JobId = @JobId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Job deleted successfully." : "Failed to delete job."
        };
    }
}
