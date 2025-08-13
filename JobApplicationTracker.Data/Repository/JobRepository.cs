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
                   JobType,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   JobEmpolymentType,
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

    public async Task<JobsDataModel?> GetJobsByIdAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
            SELECT JobId,
                   CompanyId,
                   PostedByUserId,
                   JobType,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   JobEmpolymentType,
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

    // ✅ NEW METHOD: Get Jobs By Company ID
    public async Task<IEnumerable<JobsDataModel>> GetJobsByCompanyIdAsync(int companyId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
            SELECT JobId,
                   CompanyId,
                   PostedByUserId,
                   JobType,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   JobEmpolymentType,
                   ExperienceLevel,
                   Responsibilities,
                   Requirements,
                   Benefits,
                   PostedAt,
                   ApplicationDeadline,
                   Status,
                   Views
            FROM Job
            WHERE CompanyId = @CompanyId
            ORDER BY PostedAt DESC
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companyId, DbType.Int32);

        return await connection.QueryAsync<JobsDataModel>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitJobAsync(JobsDataModel jobsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        var isNewJob = jobsDto.JobId <= 0;

        var parameters = new DynamicParameters();
        parameters.Add("CompanyId", jobsDto.CompanyId, DbType.Int32);
        parameters.Add("PostedByUserId", jobsDto.PostedByUserId, DbType.Int32);
        parameters.Add("JobType", jobsDto.JobType, DbType.String);
        parameters.Add("Description", jobsDto.Description, DbType.String);
        parameters.Add("Location", jobsDto.Location, DbType.String);
        parameters.Add("SalaryRangeMin", jobsDto.SalaryRangeMin, DbType.Decimal);
        parameters.Add("SalaryRangeMax", jobsDto.SalaryRangeMax, DbType.Decimal);
        parameters.Add("JobType", jobsDto.JobEmpolymentType, DbType.Int32);
        parameters.Add("ExperienceLevel", jobsDto.ExperienceLevel, DbType.Int32);
        parameters.Add("Responsibilities", jobsDto.Responsibilities, DbType.String);
        parameters.Add("Requirements", jobsDto.Requirements, DbType.String);
        parameters.Add("Benefits", jobsDto.Benefits, DbType.String);
        parameters.Add("PostedAt", jobsDto.PostedAt, DbType.DateTime);
        parameters.Add("ApplicationDeadline", jobsDto.ApplicationDeadline, DbType.DateTime);
        parameters.Add("Status", jobsDto.Status, DbType.String);
        parameters.Add("Views", jobsDto.Views, DbType.Int32);
        parameters.Add("JobId", jobsDto.JobId, DbType.Int32);

        if (isNewJob)
        {
            var insertQuery = @"
            INSERT INTO Job (
                CompanyId, PostedByUserId, JobType, Description, Location,
                SalaryRangeMin, SalaryRangeMax, JobEmpolymentType, ExperienceLevel,
                Responsibilities, Requirements, Benefits, PostedAt,
                ApplicationDeadline, Status, Views
            )
            VALUES (
                @CompanyId, @PostedByUserId, @JobType, @Description, @Location,
                @SalaryRangeMin, @SalaryRangeMax, @JobEmpolymentType, @ExperienceLevel,
                @Responsibilities, @Requirements, @Benefits, @PostedAt,
                @ApplicationDeadline, @Status, @Views
            );
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var newJobId = await connection.ExecuteScalarAsync<int>(insertQuery, parameters);
            return new ResponseDto
            {
                IsSuccess = newJobId > 0,
                Message = newJobId > 0 ? "Job inserted successfully." : "Failed to insert job.",
                StatusCode = newJobId > 0 ? 200 : 400,
                Id = newJobId
            };
        }
        else
        {
            var updateQuery = @"
            UPDATE Job
            SET CompanyId = @CompanyId, PostedByUserId = @PostedByUserId,
                JobType = @JobType, Description = @Description, Location = @Location,
                SalaryRangeMin = @SalaryRangeMin, SalaryRangeMax = @SalaryRangeMax,
                JobEmpolymentType = @JobEmpolymentType, ExperienceLevel = @ExperienceLevel,
                Responsibilities = @Responsibilities, Requirements = @Requirements,
                Benefits = @Benefits, PostedAt = @PostedAt,
                ApplicationDeadline = @ApplicationDeadline, Status = @Status,
                Views = @Views
            WHERE JobId = @JobId";

            var rowsAffected = await connection.ExecuteAsync(updateQuery, parameters);
            return new ResponseDto
            {
                IsSuccess = rowsAffected > 0,
                Message = rowsAffected > 0 ? "Job updated successfully." : "Failed to update job.",
                StatusCode = rowsAffected > 0 ? 200 : 400,
                Id = rowsAffected > 0 ? jobsDto.JobId : 0
            };
        }
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
            Message = affectedRows > 0 ? "Job deleted successfully." : "Failed to delete job.",
            Id = jobId  // Add this line to return the deleted job ID
        };
    }
}