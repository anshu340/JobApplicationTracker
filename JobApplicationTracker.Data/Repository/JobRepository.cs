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
                   PostedByUserId,
                   Title,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   JobTypeId,
                   ExperienceLevel,
                   Responsibilities,
                   Requirements,
                   Benefits,
                   PostedAt,
                   ApplicationDeadline,
                   Status,
                   Views
            FROM Job
            """;

        return await connection.QueryAsync<JobsDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<JobsDataModel?> GetJobsByIdAsync(int jobId) // ✅ Fixed return type
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
            SELECT JobId,
                   CompanyId,
                   PostedByUserId,
                   Title,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   JobTypeId,
                   ExperienceLevel,
                   Responsibilities,
                   Requirements,
                   Benefits,
                   PostedAt,
                   ApplicationDeadline,
                   Status,
                   Views
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
            // Insert new job - ✅ Fixed column mapping
            sql = """
                INSERT INTO Job (
                    CompanyId,
                    PostedByUserId,
                    Title,
                    Description,
                    Location,
                    SalaryRangeMin,
                    SalaryRangeMax,
                    JobTypeId,
                    ExperienceLevel,
                    Responsibilities,
                    Requirements,
                    Benefits,
                    PostedAt,
                    ApplicationDeadline,
                    Status,
                    Views
                )
                VALUES (
                    @CompanyId,
                    @PostedByUserId,
                    @Title,
                    @Description,
                    @Location,
                    @SalaryRangeMin,
                    @SalaryRangeMax,
                    @JobTypeId,
                    @ExperienceLevel,
                    @Responsibilities,
                    @Requirements,
                    @Benefits,
                    @PostedAt,
                    @ApplicationDeadline,
                    @Status,
                    @Views
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;
        }
        else
        {
            // Update existing job - ✅ Fixed column mapping
            sql = """
                UPDATE Job
                SET
                    CompanyId = @CompanyId,
                    PostedByUserId = @PostedByUserId,
                    Title = @Title,
                    Description = @Description,
                    Location = @Location,
                    SalaryRangeMin = @SalaryRangeMin,
                    SalaryRangeMax = @SalaryRangeMax,
                    JobTypeId = @JobTypeId,
                    ExperienceLevel = @ExperienceLevel,
                    Responsibilities = @Responsibilities,
                    Requirements = @Requirements,
                    Benefits = @Benefits,
                    PostedAt = @PostedAt,
                    ApplicationDeadline = @ApplicationDeadline,
                    Status = @Status,
                    Views = @Views
                WHERE JobId = @JobId
                """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobsDto.JobId, DbType.Int32);
        parameters.Add("@CompanyId", jobsDto.CompanyId, DbType.Int32); // ✅ Fixed parameter name
        parameters.Add("@PostedByUserId", jobsDto.PostedByUserId, DbType.Int32); // ✅ Fixed parameter
        parameters.Add("@Title", jobsDto.Title, DbType.String);
        parameters.Add("@Description", jobsDto.Description, DbType.String);
        parameters.Add("@Location", jobsDto.Location, DbType.String);
        parameters.Add("@SalaryRangeMin", jobsDto.SalaryRangeMin, DbType.Decimal);
        parameters.Add("@SalaryRangeMax", jobsDto.SalaryRangeMax, DbType.Decimal);
        parameters.Add("@JobTypeId", jobsDto.JobTypeId, DbType.Int32);
        parameters.Add("@ExperienceLevel", jobsDto.ExperienceLevel, DbType.Int32);
        parameters.Add("@Responsibilities", jobsDto.Responsibilities, DbType.String); // ✅ Added missing
        parameters.Add("@Requirements", jobsDto.Requirements, DbType.String);
        parameters.Add("@Benefits", jobsDto.Benefits, DbType.String); // ✅ Added missing
        parameters.Add("@PostedAt", jobsDto.PostedAt, DbType.DateTime);
        parameters.Add("@ApplicationDeadline", jobsDto.ApplicationDeadline, DbType.DateTime);
        parameters.Add("@Status", jobsDto.Status, DbType.String); // ✅ Fixed: String not Boolean
        parameters.Add("@Views", jobsDto.Views, DbType.Int32); // ✅ Added missing

        int affectedRows;

        if (jobsDto.JobId <= 0)
        {
            var newId = await connection.ExecuteScalarAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            jobsDto.JobId = newId;
        }
        else
        {
            // ✅ Fixed validation logic
            if (jobsDto.JobId <= 0)
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

        var sql = """DELETE FROM Job WHERE JobId = @JobId""";

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