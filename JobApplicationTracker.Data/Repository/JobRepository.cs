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
              SELECT NotificationId, 
                     UserId, 
                     Title, 
                     Message, 
                     NotificationTypeId,    
                     IsRead, 
                     CreatedAt 
              FROM Notifications
              """;

        return await connection.QueryAsync<JobsDataModel>(sql).ConfigureAwait(false);
    }
    

    public async Task<JobsDataModel> GetJobsByIdAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to fetch a job by ID
        var sql = """
              SELECT JobId, 
                     CompanyUserId,
                     Title,
                     Description,
                     Requirements,
                     Location,
                     JobTypeId,
                     SalaryMin,
                     SalaryMax,
                     ExperienceLevelId,
                     IsActive,
                     CreatedAt,
                     UpdatedAt,
                     Deadline
              FROM Jobs
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
                    INSERT INTO Jobs (CompanyUserId, Title, Description, Requirements, Location, JobTypeId, SalaryMin, SalaryMax, ExperienceLevel, IsActive, CreatedAt, UpdatedAt, Deadline)
                    VALUES (@CompanyUserId, @Title, @Description, @Requirements, @Location, @JobTypeId, @SalaryMin, @SalaryMax, @ExperienceLevel, @IsActive, @CreatedAt, @UpdatedAt, @Deadline);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                    """;
        }
        else
        {
            // Update existing job
            sql = """
                    UPDATE Jobs
                    SET 
                        CompanyUserId = @CompanyUserId,
                        Title = @Title,
                        Description = @Description,
                        Requirements = @Requirements,
                        Location = @Location,
                        JobTypeId = @JobTypeId,
                        SalaryMin = @SalaryMin,
                        SalaryMax = @SalaryMax,
                        ExperienceLevel = @ExperienceLevel,
                        IsActive = @IsActive,
                        UpdatedAt = @UpdatedAt,
                        Deadline = @Deadline
                    WHERE JobId = @JobId
                    """;
        }

            var parameters = new DynamicParameters();
            parameters.Add("@JobId", jobsDto.JobId, DbType.Int32);
            parameters.Add("@CompanyUserId", jobsDto.PostedByUserId, DbType.Int32);
            parameters.Add("@Title", jobsDto.Title, DbType.String);
            parameters.Add("@Description", jobsDto.Description, DbType.String);
            parameters.Add("@Requirements", jobsDto.Requirements, DbType.String);
            parameters.Add("@Location", jobsDto.Location, DbType.String);
            parameters.Add("@JobTypeId", jobsDto.JobTypeId, DbType.Int32);
            parameters.Add("@SalaryMin", jobsDto.SalaryRangeMin, DbType.Decimal);
            parameters.Add("@SalaryMax", jobsDto.SalaryRangeMax, DbType.Decimal);
            parameters.Add("@ExperienceLevel", jobsDto.ExperienceLevel, DbType.Int32);
            parameters.Add("@IsActive", jobsDto.Status, DbType.Boolean);
            parameters.Add("@CreatedAt", jobsDto.PostedAt, DbType.DateTime);
            parameters.Add("@UpdatedAt", DateTime.UtcNow, DbType.DateTime);
            parameters.Add("@Deadline", jobsDto.ApplicationDeadline, DbType.DateTime);

            var affectedRows = 0;

        if (jobsDto.JobId <= 0)
        {
            // Insert operation
            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            jobsDto.JobId = newId; // Set the ID for the newly inserted record
        }
        else
        {
            // Update operation
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Jobs submitted successfully." : "Failed to submit job."
        };
    }



    public async Task<ResponseDto> DeleteJobAsync(int jobId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a job by ID
        var sql = """DELETE FROM Jobs WHERE JobId = @JobId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobId", jobId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete job."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Jobs deleted successfully."
        };
    }
}