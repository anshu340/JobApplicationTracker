using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

public class JobSeekerEducationRepository : IJobSeekersEducationRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public JobSeekerEducationRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<JobSeekerEducation>> GetAllJobSeekerEducationAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT EducationId, 
                         JobSeekerId,
                         Institution,
                         Degree,
                         FieldOfStudy,
                         StartDate,
                         EndDate,
                         GPA
                          
                  FROM JobSeekerEducation
                  """;

        return await connection.QueryAsync<JobSeekerEducation>(sql).ConfigureAwait(false);
    }

    public async Task<JobSeekerEducation> GetJobSeekerEducationByIdAsync(int jobSeekerEducationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to fetch job seeker education by ID
        var sql = """
              SELECT EducationId, 
                     JobSeekerId,
                     Institution,
                     Degree,
                     FieldOfStudy,
                     StartDate,
                     EndDate,
                     GPA 
              FROM JobSeekerEducation
              WHERE EducationId = @EducationId
              """;

        var parameters = new DynamicParameters();
        parameters.Add("@EducationId", jobSeekerEducationId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobSeekerEducation>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitJobSeekerEducationAsync(JobSeekerEducation jobSeekerEducationDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (jobSeekerEducationDto.EducationId <= 0)
        {
            // Insert new job seeker education
            sql = """
        INSERT INTO JobSeekerEducation (JobSeekerId, Institution, Degree, FieldOfStudy, StartDate, EndDate, GPA)
        VALUES (@JobSeekerId, @Institution, @Degree, @FieldOfStudy, @StartDate, @EndDate, @GPA);
        """;
        }
        else
        {
            // Update existing job seeker education
            sql = """
        UPDATE JobSeekerEducation
        SET 
            Institution = @Institution,
            Degree = @Degree,
            FieldOfStudy = @FieldOfStudy,
            StartDate = @StartDate,
            EndDate = @EndDate,
            GPA = @GPA
        WHERE EducationId = @EducationId
        """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@EducationId", jobSeekerEducationDto.EducationId, DbType.Int32);
        parameters.Add("@JobSeekerId", jobSeekerEducationDto.JobSeekerId, DbType.Int32);
        parameters.Add("@University", jobSeekerEducationDto.University, DbType.String);
        parameters.Add("@Degree", jobSeekerEducationDto.Degree, DbType.String);
        parameters.Add("@FieldOfStudy", jobSeekerEducationDto.FieldOfStudy, DbType.String);
        parameters.Add("@StartDate", jobSeekerEducationDto.StartDate, DbType.DateTime);
        parameters.Add("@EndDate", jobSeekerEducationDto.EndDate, DbType.DateTime);
        parameters.Add("@GPA", jobSeekerEducationDto.Gpa, DbType.Decimal);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Jobs seeker education submitted successfully." : "Failed to submit job seeker education."
        };
    }



    public async Task<ResponseDto> DeleteJobSeekerEducationAsync(int educationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a job seeker education by ID
        var sql = """DELETE FROM JobSeekerEducation WHERE EducationId = @EducationId""";

        var parameters = new DynamicParameters();
        parameters.Add("@EducationId", educationId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete job seeker education."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Jobs seeker education deleted successfully."
        };
    }
}