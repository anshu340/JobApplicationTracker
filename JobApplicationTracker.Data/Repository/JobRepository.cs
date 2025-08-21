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

    // ✅ New method to delete inactive jobs older than 1 day (for user-facing endpoints)
    private async Task DeleteExpiredInactiveJobsAsync(IDbConnection connection)
    {
        var deleteExpiredInactiveSql = """
            DELETE FROM Job 
            WHERE Status = 'I' 
            AND ApplicationDeadline < DATEADD(DAY, -1, CAST(GETDATE() AS DATE))
            """;

        await connection.ExecuteAsync(deleteExpiredInactiveSql).ConfigureAwait(false);
    }

    // ✅ Updated method for user-facing job listings (with auto-delete) - ONLY PUBLISHED JOBS
    public async Task<IEnumerable<JobsDataModel>> GetAllJobsAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // ✅ First update expired jobs to inactive status
        var updateExpiredSql = """
            UPDATE Job 
            SET Status = 'I' 
            WHERE Status = 'A' 
            AND ApplicationDeadline < CAST(GETDATE() AS DATE)
            """;

        await connection.ExecuteAsync(updateExpiredSql).ConfigureAwait(false);

        // ✅ Delete inactive jobs older than 1 day (USER PAGE ONLY)
        await DeleteExpiredInactiveJobsAsync(connection);

        var sql = """
            SELECT JobId,
                   CompanyId,
                   PostedByUserId,
                   JobType,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   EmpolymentType,
                   ExperienceLevel,
                   Responsibilities,
                   Requirements,
                   Benefits,
                   PostedAt,
                   ApplicationDeadline,
                   Status,
                   Views,
                   Skills,
                   IsPublished
            FROM Job
            WHERE IsPublished = 1
            ORDER BY PostedAt DESC
            """;

        return await connection.QueryAsync<JobsDataModel>(sql).ConfigureAwait(false);
    }

    // ✅ Updated method for user-facing single job view (with auto-delete check) - ONLY PUBLISHED JOBS
    public async Task<JobsDataModel?> GetJobsByIdAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // ✅ Update this specific job if expired
        var updateExpiredSql = """
            UPDATE Job 
            SET Status = 'I' 
            WHERE JobId = @JobId 
            AND Status = 'A' 
            AND ApplicationDeadline < CAST(GETDATE() AS DATE)
            """;

        var updateParameters = new DynamicParameters();
        updateParameters.Add("@JobId", jobId, DbType.Int32);
        await connection.ExecuteAsync(updateExpiredSql, updateParameters).ConfigureAwait(false);

        // ✅ Delete this specific job if it's been inactive for more than 1 day (USER PAGE ONLY)
        var deleteExpiredInactiveSql = """
            DELETE FROM Job 
            WHERE JobId = @JobId 
            AND Status = 'I' 
            AND ApplicationDeadline < DATEADD(DAY, -1, CAST(GETDATE() AS DATE))
            """;

        await connection.ExecuteAsync(deleteExpiredInactiveSql, updateParameters).ConfigureAwait(false);

        var sql = """
            SELECT JobId,
                   CompanyId,
                   PostedByUserId,
                   JobType,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   EmpolymentType,
                   ExperienceLevel,
                   Responsibilities,
                   Requirements,
                   Benefits,
                   PostedAt,
                   ApplicationDeadline,
                   Status,
                   Views,
                   Skills,
                   IsPublished
            FROM Job
            WHERE JobId = @JobId
            AND IsPublished = 1
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobsDataModel>(sql, parameters).ConfigureAwait(false);
    }

    // ✅ Company-facing method (NO AUTO-DELETE - companies can see ALL their jobs including unpublished)
    public async Task<IEnumerable<JobsDataModel>> GetJobsByCompanyIdAsync(int companyId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // ✅ Only update expired jobs to inactive status, but DON'T DELETE them
        var updateExpiredSql = """
            UPDATE Job 
            SET Status = 'I' 
            WHERE CompanyId = @CompanyId 
            AND Status = 'A' 
            AND ApplicationDeadline < CAST(GETDATE() AS DATE)
            """;

        var updateParameters = new DynamicParameters();
        updateParameters.Add("@CompanyId", companyId, DbType.Int32);
        await connection.ExecuteAsync(updateExpiredSql, updateParameters).ConfigureAwait(false);

        // ✅ Fetch all jobs for the company (including inactive and unpublished ones)
        var sql = """
            SELECT JobId,
                   CompanyId,
                   PostedByUserId,
                   JobType,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   EmpolymentType,
                   ExperienceLevel,
                   Responsibilities,
                   Requirements,
                   Benefits,
                   PostedAt,
                   ApplicationDeadline,
                   Status,
                   Views,
                   Skills,
                   IsPublished
            FROM Job
            WHERE CompanyId = @CompanyId
            ORDER BY PostedAt DESC
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companyId, DbType.Int32);

        return await connection.QueryAsync<JobsDataModel>(sql, parameters).ConfigureAwait(false);
    }

    // ✅ New method specifically for user-facing job search (with auto-delete) - ONLY PUBLISHED JOBS
    public async Task<IEnumerable<JobsDataModel>> GetActiveJobsForUsersAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // ✅ Update expired jobs to inactive status
        var updateExpiredSql = """
            UPDATE Job 
            SET Status = 'I' 
            WHERE Status = 'A' 
            AND ApplicationDeadline < CAST(GETDATE() AS DATE)
            """;

        await connection.ExecuteAsync(updateExpiredSql).ConfigureAwait(false);

        // ✅ Delete inactive jobs older than 1 day
        await DeleteExpiredInactiveJobsAsync(connection);

        // ✅ Only return active AND published jobs for users
        var sql = """
            SELECT JobId,
                   CompanyId,
                   PostedByUserId,
                   JobType,
                   Description,
                   Location,
                   SalaryRangeMin,
                   SalaryRangeMax,
                   EmpolymentType,
                   ExperienceLevel,
                   Responsibilities,
                   Requirements,
                   Benefits,
                   PostedAt,
                   ApplicationDeadline,
                   Status,
                   Views,
                   Skills,
                   IsPublished
            FROM Job
            WHERE Status = 'A' AND IsPublished = 1
            ORDER BY PostedAt DESC
            """;

        return await connection.QueryAsync<JobsDataModel>(sql).ConfigureAwait(false);
    }

    // ✅ NEW METHOD: Publish a job
    public async Task<ResponseDto> PublishJobAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
            UPDATE Job 
            SET IsPublished = 1 
            WHERE JobId = @JobId
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobId, DbType.Int32);

        var rowsAffected = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "Job published successfully." : "Job not found or failed to publish.",
            StatusCode = rowsAffected > 0 ? 200 : 404,
            Id = jobId
        };
    }

    // ✅ NEW METHOD: Unpublish a job
    public async Task<ResponseDto> UnpublishJobAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
            UPDATE Job 
            SET IsPublished = 0 
            WHERE JobId = @JobId
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobId, DbType.Int32);

        var rowsAffected = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "Job unpublished successfully." : "Job not found or failed to unpublish.",
            StatusCode = rowsAffected > 0 ? 200 : 404,
            Id = jobId
        };
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
        parameters.Add("EmpolymentType", jobsDto.EmpolymentType, DbType.String);
        parameters.Add("ExperienceLevel", jobsDto.ExperienceLevel, DbType.String);
        parameters.Add("Responsibilities", jobsDto.Responsibilities, DbType.String);
        parameters.Add("Requirements", jobsDto.Requirements, DbType.String);
        parameters.Add("Benefits", jobsDto.Benefits, DbType.String);
        parameters.Add("PostedAt", jobsDto.PostedAt, DbType.DateTime);
        parameters.Add("ApplicationDeadline", jobsDto.ApplicationDeadline, DbType.DateTime);
        parameters.Add("Status", jobsDto.Status, DbType.String);
        parameters.Add("Views", jobsDto.Views, DbType.Int32);
        parameters.Add("Skills", jobsDto.Skills, DbType.String);
        parameters.Add("IsPublished", jobsDto.IsPublished, DbType.Boolean); // ✅ Add IsPublished

        if (isNewJob)
        {
            var insertQuery = """
                INSERT INTO Job (
                    CompanyId, PostedByUserId, JobType, Description, Location,
                    SalaryRangeMin, SalaryRangeMax, EmpolymentType, ExperienceLevel,
                    Responsibilities, Requirements, Benefits, PostedAt,
                    ApplicationDeadline, Status, Views, Skills, IsPublished
                )
                VALUES (
                    @CompanyId, @PostedByUserId, @JobType, @Description, @Location,
                    @SalaryRangeMin, @SalaryRangeMax, @EmpolymentType, @ExperienceLevel,
                    @Responsibilities, @Requirements, @Benefits, @PostedAt,
                    @ApplicationDeadline, @Status, @Views, @Skills, @IsPublished
                );
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            var newJobId = await connection.ExecuteScalarAsync<int>(insertQuery, parameters).ConfigureAwait(false);
            return new ResponseDto
            {
                IsSuccess = newJobId > 0,
                Message = newJobId > 0 ? "Job created successfully." : "Failed to create job.",
                StatusCode = newJobId > 0 ? 201 : 400,
                Id = newJobId
            };
        }
        else
        {
            parameters.Add("JobId", jobsDto.JobId, DbType.Int32);

            var updateQuery = """
                UPDATE Job
                SET CompanyId = @CompanyId, PostedByUserId = @PostedByUserId,
                    JobType = @JobType, Description = @Description, Location = @Location,
                    SalaryRangeMin = @SalaryRangeMin, SalaryRangeMax = @SalaryRangeMax,
                    EmpolymentType = @EmpolymentType, ExperienceLevel = @ExperienceLevel,
                    Responsibilities = @Responsibilities, Requirements = @Requirements,
                    Benefits = @Benefits, PostedAt = @PostedAt,
                    ApplicationDeadline = @ApplicationDeadline, Status = @Status,
                    Views = @Views, Skills = @Skills, IsPublished = @IsPublished
                WHERE JobId = @JobId
                """;

            var rowsAffected = await connection.ExecuteAsync(updateQuery, parameters).ConfigureAwait(false);
            return new ResponseDto
            {
                IsSuccess = rowsAffected > 0,
                Message = rowsAffected > 0 ? "Job updated successfully." : "Job not found or failed to update.",
                StatusCode = rowsAffected > 0 ? 200 : 404,
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
            Message = affectedRows > 0 ? "Job deleted successfully." : "Job not found or failed to delete.",
            StatusCode = affectedRows > 0 ? 200 : 404,
            Id = jobId
        };
    }
}