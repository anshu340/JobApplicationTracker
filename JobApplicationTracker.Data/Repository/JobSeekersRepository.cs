using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

public class JobSeekerRepository : IJobSeekersRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public JobSeekerRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<JobSeekersDataModel>> GetAllJobSeekersAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT JobSeekerId, 
                         FirstName,
                         LastName,
                         Phone,
                         Location,
                         DateOfBirth,
                         ProfilePicture,
                         Resume,
                         Bio,
                         CreatedAt,
                         UpdatedAt

                  FROM JobSeekers
                  """;




        return await connection.QueryAsync<JobSeekersDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<JobSeekersDataModel> GetJobSeekersByIdAsync(int jobSeekerId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        // SQL query to fetch a job seeker by ID
        var sql = """
              SELECT JobSeekerId, 
                     FirstName,
                     LastName,
                     Phone,
                     Location,
                     DateOfBirth,
                     ProfilePicture,
                     Resume,
                     Bio,
                     CreatedAt,
                     UpdatedAt
              FROM JobSeekers
              WHERE JobSeekerId = @JobSeekerId
              """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerId", jobSeekerId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobSeekersDataModel>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitJobSeekersAsync(JobSeekersDataModel jobSeekerDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (jobSeekerDto.JobSeekerId <= 0)
        {
            // Insert new job seeker (assumes JobSeekerId is auto-incremented)
            sql = """
        INSERT INTO JobSeekers (FirstName, LastName, Phone, Location, DateOfBirth, ProfilePicture, Resume, Bio, CreatedAt, UpdatedAt)
        VALUES (@FirstName, @LastName, @Phone, @Location, @DateOfBirth, @ProfilePicture, @Resume, @Bio, @CreatedAt, @UpdatedAt);
        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;
        }
        else
        {
            // Update existing job seeker
            sql = """
        UPDATE JobSeekers
        SET 
            FirstName = @FirstName,
            LastName = @LastName,
            Phone = @Phone,
            Location = @Location,
        WHERE JobSeekerId = @JobSeekerId
        """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerId", jobSeekerDto.JobSeekerId, DbType.Int32);
        parameters.Add("@FirstName", jobSeekerDto.FirstName, DbType.String);
        parameters.Add("@LastName", jobSeekerDto.LastName, DbType.String);
        parameters.Add("@Location", jobSeekerDto.Location, DbType.String);
        // parameters.Add("@DateOfBirth", jobSeekerDto.DateOfBirth, DbType.DateTime);
        // parameters.Add("@ProfilePicture", jobSeekerDto.ProfilePicture, DbType.String);
        // parameters.Add("@ResumeUrl", jobSeekerDto.ResumeUrl, DbType.String);
        // parameters.Add("@Bio", jobSeekerDto.Bio, DbType.String);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Jobs type submitted successfully." : "Failed to submit job type."
        };
    }

    public async Task<ResponseDto> CreateJobSeekersAsync(JobSeekersDataModel reqeust)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        string sql = @"INSERT INTO JobSeekers (UserId, FirstName, LastName, Location)
                     VALUES (@UserId, @FirstName, @LastName, @Location);
        SELECT CAST(SCOPE_IDENTITY() AS INT)";
        
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", reqeust.UserId, DbType.Int32);
        parameters.Add("@FirstName", reqeust.FirstName, DbType.String);
        parameters.Add("@LastName", reqeust.LastName, DbType.String);
        parameters.Add("@Location", reqeust.Location, DbType.String);
        // parameters.Add("@ProfilePicture", reqeust.ProfilePicture, DbType.String);
        // parameters.Add("@ResumeUrl", reqeust.ResumeUrl, DbType.String);
        // parameters.Add("@PortfolioUrl", reqeust.PortfolioUrl, DbType.String);
        // parameters.Add("@LinkedinProfile", reqeust.LinkedinProfile, DbType.String);
        // parameters.Add("@Headline", reqeust.Headline, DbType.String);
        // parameters.Add("@Bio", reqeust.Bio, DbType.String);
        // parameters.Add("@DateOfBirth", reqeust.DateOfBirth, DbType.DateTime);
        // parameters.Add("@PreferredJobTypes", reqeust.PreferredJobTypes, DbType.String);
        // parameters.Add("@PreferredExperienceLevels", reqeust.PreferredExperienceLevels, DbType.String);
        
        int affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        
        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Job Seeker Created successfully." : "Failed to create user of type jobseeker."
        };
        
    }
    public async Task<ResponseDto> DeleteJobSeekersAsync(int jobSeekerId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a job seeker by ID
        var sql = """DELETE FROM JobSeekers WHERE JobSeekerId = @JobSeekerId""";

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerId", jobSeekerId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete job seeker."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Jobs seeker deleted successfully."
        };
    }
}