using Dapper;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Dto;
using System.Data;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

public class JobSeekerEducationRepository : IJobSeekersEducationRepository
{
    private readonly IDatabaseConnectionService _connectionService;

    public JobSeekerEducationRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<IEnumerable<JobSeekerEducationDto>> GetAllJobSeekerEducationAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT EducationId, 
                         JobSeekerId,
                         University,
                         College,
                         Degree,
                         FieldOfStudy,
                         StartDate,
                         Status,
                         EndDate,
                         GPA
                  FROM JobSeekerEducation
                  """;

        return await connection.QueryAsync<JobSeekerEducationDto>(sql).ConfigureAwait(false);
    }

    public async Task<JobSeekerEducationDto> GetJobSeekerEducationByIdAsync(int jobSeekerEducationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT EducationId, 
                         JobSeekerId,
                         University,
                         College,
                         Degree,
                         FieldOfStudy,
                         StartDate,
                         Status,
                         EndDate,
                         GPA 
                  FROM JobSeekerEducation
                  WHERE EducationId = @EducationId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@EducationId", jobSeekerEducationId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<JobSeekerEducationDto>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<IEnumerable<JobSeekerEducationDto>> GetJobSeekerEducationByJobSeekerIdAsync(int jobSeekerId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT EducationId, 
                         JobSeekerId,
                         University,
                         College,
                         Degree,
                         FieldOfStudy,
                         StartDate,
                         Status,
                         EndDate,
                         GPA 
                  FROM JobSeekerEducation
                  WHERE JobSeekerId = @JobSeekerId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@JobSeekerId", jobSeekerId, DbType.Int32);

        return await connection.QueryAsync<JobSeekerEducationDto>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitJobSeekerEducationAsync(JobSeekerEducationDto jobSeekerEducationDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var isNewEducation = jobSeekerEducationDto.EducationId <= 0;
        var parameters = new DynamicParameters();

        // Common parameters for both insert and update
        parameters.Add("@JobSeekerId", jobSeekerEducationDto.JobSeekerId, DbType.Int32);
        parameters.Add("@University", jobSeekerEducationDto.University, DbType.String);
        parameters.Add("@College", jobSeekerEducationDto.College, DbType.String);
        parameters.Add("@Degree", jobSeekerEducationDto.Degree, DbType.String);
        parameters.Add("@FieldOfStudy", jobSeekerEducationDto.FieldOfStudy, DbType.String);
        parameters.Add("@StartDate", jobSeekerEducationDto.StartDate, DbType.DateTime);
        parameters.Add("@Status", jobSeekerEducationDto.Status, DbType.String);
        parameters.Add("@EndDate", jobSeekerEducationDto.EndDate, DbType.DateTime);
        parameters.Add("@GPA", jobSeekerEducationDto.Gpa, DbType.Double);

        if (isNewEducation)
        {
            var insertSql = """
                           INSERT INTO JobSeekerEducation 
                           (JobSeekerId, University, College, Degree, FieldOfStudy, StartDate, Status, EndDate, GPA)
                           VALUES 
                           (@JobSeekerId, @University, @College, @Degree, @FieldOfStudy, @StartDate, @Status, @EndDate, @GPA);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);
                           """;

            var newEducationId = await connection.ExecuteScalarAsync<int>(insertSql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = newEducationId > 0,
                Message = newEducationId > 0 ? "Job seeker education inserted successfully." : "Failed to insert job seeker education.",
                StatusCode = newEducationId > 0 ? 201 : 400,
                Id = newEducationId
            };
        }
        else
        {
            // First check if the record exists
            var existingRecord = await GetJobSeekerEducationByIdAsync(jobSeekerEducationDto.EducationId);
            if (existingRecord == null)
            {
                return new ResponseDto
                {
                    Id = 0,
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = $"Education record with ID {jobSeekerEducationDto.EducationId} not found"
                };
            }

            // Add EducationId parameter for update
            parameters.Add("@EducationId", jobSeekerEducationDto.EducationId, DbType.Int32);

            var updateSql = """
                           UPDATE JobSeekerEducation
                           SET JobSeekerId = @JobSeekerId,
                               University = @University,
                               College = @College,
                               Degree = @Degree,
                               FieldOfStudy = @FieldOfStudy,
                               StartDate = @StartDate,
                               Status = @Status,
                               EndDate = @EndDate,
                               GPA = @GPA
                           WHERE EducationId = @EducationId
                           """;

            var rowsAffected = await connection.ExecuteAsync(updateSql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = rowsAffected > 0,
                Message = rowsAffected > 0 ? "Job seeker education updated successfully." : "Failed to update job seeker education.",
                StatusCode = rowsAffected > 0 ? 200 : 400,
                Id = rowsAffected > 0 ? jobSeekerEducationDto.EducationId : 0
            };
        }
    }

    public async Task<ResponseDto> DeleteJobSeekerEducationAsync(int educationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // First check if the record exists
        var existingRecord = await GetJobSeekerEducationByIdAsync(educationId);
        if (existingRecord == null)
        {
            return new ResponseDto
            {
                Id = 0,
                IsSuccess = false,
                StatusCode = 404,
                Message = "Education record not found"
            };
        }

        var sql = "DELETE FROM JobSeekerEducation WHERE EducationId = @EducationId";

        var parameters = new DynamicParameters();
        parameters.Add("@EducationId", educationId, DbType.Int32);

        var rowsAffected = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "Job seeker education deleted successfully." : "Failed to delete job seeker education.",
            StatusCode = rowsAffected > 0 ? 200 : 400,
            Id = rowsAffected > 0 ? educationId : 0
        };
    }
}