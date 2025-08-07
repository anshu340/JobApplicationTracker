using Dapper;
using JobApplicationTracker.Data.Interface;
using JobApplicationTracker.Dto;
using System.Data;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

public class UsersEducationRepository : IUsersEducationRepository
{
    private readonly IDatabaseConnectionService _connectionService;

    public UsersEducationRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }

    public async Task<IEnumerable<UsersEducationDto>> GetAllUsersEducationAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT EducationId, 
                         UsersId,
                         University,
                         College,
                         Degree,
                         FieldOfStudy,
                         StartDate,
                         Status,
                         EndDate,
                         GPA
                  FROM UsersEducation
                  """;

        return await connection.QueryAsync<UsersEducationDto>(sql).ConfigureAwait(false);
    }

    public async Task<UsersEducationDto> GetUsersEducationByIdAsync(int usersEducationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT EducationId, 
                         UsersId,
                         University,
                         College,
                         Degree,
                         FieldOfStudy,
                         StartDate,
                         Status,
                         EndDate,
                         GPA 
                  FROM UsersEducation
                  WHERE EducationId = @EducationId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@EducationId", usersEducationId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<UsersEducationDto>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<IEnumerable<UsersEducationDto>> GetUsersEducationByUserIdAsync(int userId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT EducationId, 
                         UsersId,
                         University,
                         College,
                         Degree,
                         FieldOfStudy,
                         StartDate,
                         Status,
                         EndDate,
                         GPA 
                  FROM UsersEducation
                  WHERE UserId = @UserId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.Int32);

        return await connection.QueryAsync<UsersEducationDto>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitUsersEducationAsync(UsersEducationDto usersEducationDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var isNewEducation = usersEducationDto.EducationId <= 0;
        var parameters = new DynamicParameters();

        parameters.Add("@UsersId", usersEducationDto.UsersId, DbType.Int32);
        parameters.Add("@University", usersEducationDto.University, DbType.String);
        parameters.Add("@College", usersEducationDto.College, DbType.String);
        parameters.Add("@Degree", usersEducationDto.Degree, DbType.String);
        parameters.Add("@FieldOfStudy", usersEducationDto.FieldOfStudy, DbType.String);
        parameters.Add("@StartDate", usersEducationDto.StartDate, DbType.DateTime);
        parameters.Add("@Status", usersEducationDto.Status, DbType.String);
        parameters.Add("@EndDate", usersEducationDto.EndDate, DbType.DateTime);
        parameters.Add("@GPA", usersEducationDto.Gpa, DbType.Double);

        if (isNewEducation)
        {
            var insertSql = """
                           INSERT INTO UsersEducation 
                           (UsersId, University, College, Degree, FieldOfStudy, StartDate, Status, EndDate, GPA)
                           VALUES 
                           (@UsersId, @University, @College, @Degree, @FieldOfStudy, @StartDate, @Status, @EndDate, @GPA);
                           SELECT CAST(SCOPE_IDENTITY() AS INT);
                           """;

            var newEducationId = await connection.ExecuteScalarAsync<int>(insertSql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = newEducationId > 0,
                Message = newEducationId > 0 ? "User education inserted successfully." : "Failed to insert user education.",
                StatusCode = newEducationId > 0 ? 201 : 400,
                Id = newEducationId
            };
        }
        else
        {
            var existingRecord = await GetUsersEducationByIdAsync(usersEducationDto.EducationId);
            if (existingRecord == null)
            {
                return new ResponseDto
                {
                    Id = 0,
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = $"Education record with ID {usersEducationDto.EducationId} not found"
                };
            }

            parameters.Add("@EducationId", usersEducationDto.EducationId, DbType.Int32);

            var updateSql = """
                           UPDATE UsersEducation
                           SET UsersId = @UsersId,
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
                Message = rowsAffected > 0 ? "User education updated successfully." : "Failed to update user education.",
                StatusCode = rowsAffected > 0 ? 200 : 400,
                Id = rowsAffected > 0 ? usersEducationDto.EducationId : 0
            };
        }
    }

    public async Task<ResponseDto> DeleteUsersEducationAsync(int educationId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var existingRecord = await GetUsersEducationByIdAsync(educationId);
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

        var sql = "DELETE FROM UsersEducation WHERE EducationId = @EducationId";

        var parameters = new DynamicParameters();
        parameters.Add("@EducationId", educationId, DbType.Int32);

        var rowsAffected = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "User education deleted successfully." : "Failed to delete user education.",
            StatusCode = rowsAffected > 0 ? 200 : 400,
            Id = rowsAffected > 0 ? educationId : 0
        };
    }
}
