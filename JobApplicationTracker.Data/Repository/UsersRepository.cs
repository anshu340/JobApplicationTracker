using Dapper;
using JobApplicationTracker.Api.Enums;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.AuthDto;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using System.Data;

namespace JobApplicationTracker.Data.Repository;

public class UsersRepository(IDatabaseConnectionService connectionService) : IUserRepository
{
    public async Task<IEnumerable<UsersDtoResponse>> GetAllUsersAsync(int companyId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT UserId, 
                         Email, 
                         UserType,
                         PhoneNumber,
                         CreatedAt,
                         UpdatedAt
                  FROM Users where (CompanyId =@companyId or @companyId = 0)
                  """;
        var parameters = new DynamicParameters();
        parameters.Add("@companyId", companyId, DbType.Int32); // ✅ FIXED
        return await connection.QueryAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        // Step 1: Get basic user info
        var userQuery = """
            SELECT UserId, CompanyId, Email, UserType, CreatedAt, UpdatedAt
            FROM Users
            WHERE UserId = @UserId
        """;

        var user = await connection.QueryFirstOrDefaultAsync<UsersDataModel>(
            userQuery, new { UserId = userId });

        if (user is null) return null;

        var profile = new UserProfileDto
        {
            UserId = user.UserId,
            Email = user.Email,
            UserType = user.UserType,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
        // Step 2: Fetch profile based on UserType
        switch ((UserTypes)user.UserType)
        {
            case UserTypes.Company when user.CompanyId.HasValue:
                var companyQuery = "SELECT * FROM Companies WHERE CompanyId = @CompanyId";
                var company = await connection.QueryFirstOrDefaultAsync<CompanyProfileDto>(
                    companyQuery, new { CompanyId = user.CompanyId.Value });
                profile.CompanyProfile = company;
                var jobSeekerQuery = "SELECT * FROM Users WHERE UserId = @UserId";
                var jobSeeker = await connection.QueryFirstOrDefaultAsync<JobSeekersProfileDto>(
                    jobSeekerQuery, new { UserId = user.UserId });
                profile.JobSeekerProfile = jobSeeker;
                break;

            case UserTypes.JobSeeker:
                jobSeekerQuery = "SELECT * FROM Users WHERE UserId = @UserId";
                jobSeeker = await connection.QueryFirstOrDefaultAsync<JobSeekersProfileDto>(
                   jobSeekerQuery, new { UserId = user.UserId });
                profile.JobSeekerProfile = jobSeeker;
                break;

                //case UserTypes.Staff:
                //    // If you plan to fetch recruiter/staff profile later
                //    var staffQuery = "SELECT * FROM Staffs WHERE UserId = @UserId";
                //    var staff = await connection.QueryFirstOrDefaultAsync<StaffProfileDto>(
                //        staffQuery, new { UserId = user.UserId });
                //    profile.StaffProfile = staff;
                //    break;

                //case UserTypes.Admin:
                //    // Currently no specific profile to fetch for Admin, so break silently
                //    break;

                //default:
                //    // Optional: log or handle unknown UserType
                //    break;
        }



        return profile;
    }

    public async Task<UsersDtoResponse?> GetUsersByIdAsync(int usersId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
            SELECT UserId,
                   Email,
                   UserType,
                   PhoneNumber,
                   CreatedAt,
                   UpdatedAt
            FROM Users 
            WHERE UserId = @UserId
        """;

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", usersId, DbType.Int32); // ✅ FIXED

        return await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        string sql =
            userDto.UserId <= 0 ?
            @"INSERT INTO Users (Email, PasswordHash, UserType, CreatedAt, UpdatedAt) 
            VALUES (@Email, @PasswordHash, @UserType, @CreatedAt, @UpdatedAt)"
            :
            @"UPDATE Users
            SET Email = @Email,
                PasswordHash = @PasswordHash,
                UserType = @UserType,
                UpdatedAt = @UpdatedAt
            WHERE UserId = @UserId"; // ✅ FIXED trailing comma

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userDto.UserId, DbType.Int32);
        parameters.Add("Email", userDto.Email, DbType.String);
        parameters.Add("PasswordHash", userDto.PasswordHash, DbType.String);
        parameters.Add("UserType", userDto.UserType, DbType.Int32);
        parameters.Add("UpdatedAt", DateTime.UtcNow, DbType.DateTime);
        parameters.Add("CreatedAt", DateTime.UtcNow, DbType.DateTime);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "User updated successfully." : "Failed to update user."
        };
    }

    public async Task<int> CreateUserAsync(UsersDataModel userDto)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();
        var query =
             userDto.UserId <= 0 ?
             @"
            INSERT INTO Users (FirstName, LastName, Email, PasswordHash, CompanyId,
                               PhoneNumber, UserType, Location, CreatedAt, UpdatedAt) 
            VALUES (@FirstName, @LastName, @Email, @PasswordHash, @CompanyId,
                    @PhoneNumber, @UserType, @Location, @CreatedAt, @UpdatedAt);
            SELECT SCOPE_IDENTITY();"
            :
            @"UPDATE Users
              SET Email = @Email,
                 -- PasswordHash = @PasswordHash,
                  UserType = @UserType,;
                  UpdatedAt = @UpdatedAt
              WHERE UserId = @UserId";

        var parameters = new DynamicParameters();
        parameters.Add("FirstName", userDto.FirstName, DbType.String);
        parameters.Add("LastName", userDto.LastName, DbType.String);
        parameters.Add("Email", userDto.Email, DbType.String);
        parameters.Add("PasswordHash", userDto.PasswordHash, DbType.String);
        parameters.Add("CompanyId", userDto.CompanyId, DbType.Int32);
        parameters.Add("PhoneNumber", userDto.PhoneNumber, DbType.String);
        parameters.Add("UserType", userDto.UserType, DbType.Int32);
        parameters.Add("Location", userDto.Location, DbType.String);
        parameters.Add("CreatedAt", DateTime.UtcNow, DbType.DateTime);
        parameters.Add("UpdatedAt", DateTime.UtcNow, DbType.DateTime);

        return await connection.ExecuteScalarAsync<int>(query, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> DeleteUsersAsync(int userId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = "DELETE FROM Users WHERE UserId = @UserId";

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "User deleted successfully." : "Failed to delete user."
        };
    }

    public async Task<bool> DoesEmailExists(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = @"SELECT 1 FROM Users WHERE LOWER(Email) = LOWER(@Email)";
        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        var result = await connection.ExecuteScalarAsync<int?>(query, parameters).ConfigureAwait(false);
        return result.HasValue;
    }

    public async Task<UsersDtoResponse?> GetUserByPhone(string phone)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = """
            SELECT TOP 1 UserId,
                         Email,
                         UserType,
                         PhoneNumber,
                         CreatedAt,
                         UpdatedAt 
            FROM Users WHERE PhoneNumber = @PhoneNumber
        """;

        var parameters = new DynamicParameters();
        parameters.Add("@PhoneNumber", phone, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(query, parameters).ConfigureAwait(false);
    }

    public async Task<UsersDtoResponse?> GetUserByEmail(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = """
            SELECT TOP 1
                   UserId,
                   Email,
                   UserType,
                   PhoneNumber,
                   CreatedAt,
                   UpdatedAt
            FROM Users WHERE Email = @Email
        """;

        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(query, parameters).ConfigureAwait(false);
    }

    public async Task<UsersDataModel?> GetUserForLoginAsync(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = "SELECT UserId, Email, PasswordHash, UserType, FirstName, LastName FROM Users WHERE Email = @Email";
        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UsersDataModel>(query, parameters).ConfigureAwait(false);
    }
}