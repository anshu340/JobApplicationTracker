using Azure.Core;
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
                  SELECT *
                  FROM Users where (CompanyId =@companyId or @companyId = 0)
                  """;
        var parameters = new DynamicParameters();
        parameters.Add("@companyId", companyId, DbType.Int32); // ✅ FIXED
        return await connection.QueryAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<UsersProfileDto?> GetUserProfileAsync(int userId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var userQuery = """
                    SELECT * FROM Users
                    WHERE UserId = @UserId
                """;

        var user = await connection.QueryFirstOrDefaultAsync<UsersDataModel>(
            userQuery, new { UserId = userId });

        if (user is null) return null;

        var profile = new UsersProfileDto
        {
            UserId = user.UserId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            LinkedinProfile = user.LinkedinProfile,
            Location = user.Location,
            Bio = user.Bio,
            DateOfBirth = user.DateOfBirth,
            Skills = user.Skills,
            Education = user.Education,
            Experiences = user.Experiences

        };

        // ✅ FIXED: Add company profile with CompanyLogo if CompanyId > 0
        if (user.CompanyId.HasValue && user.CompanyId.Value > 0)
        {
            var companyQuery = """
            SELECT CompanyId, CompanyName, Description, WebsiteUrl, CompanyLogo, Location, ContactEmail, CreateDateTime
            FROM Companies
            WHERE CompanyId = @CompanyId
        """;

            var company = await connection.QueryFirstOrDefaultAsync<CompaniesDataModel>(
                companyQuery, new { CompanyId = user.CompanyId.Value });

            if (company is not null)
            {
                profile.CompanyProfile = new CompanyProfileDto
                {
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    WebsiteUrl = company.WebsiteUrl,
                    Location = company.Location,
                    Description = company.Description,
                    CompanyLogo = company.CompanyLogo, // ✅ ADDED: Include company logo
                    ContactEmail = company.ContactEmail // ✅ ADDED: Include contact email
                };
            }
        }

        return profile;
    }

    public async Task<UsersDtoResponse?> GetUsersByIdAsync(int usersId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
                SELECT*
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
        var isNewUser = userDto.UserId <= 0;
        var parameters = new DynamicParameters();

        if (isNewUser)
        {
            // For new users, set defaults for null values if needed
            parameters.Add("FirstName", userDto.FirstName ?? "");
            parameters.Add("LastName", userDto.LastName ?? "");
            parameters.Add("Email", userDto.Email ?? "");
            parameters.Add("PasswordHash", userDto.PasswordHash ?? "");
            parameters.Add("CompanyId", userDto.CompanyId);
            parameters.Add("PhoneNumber", userDto.PhoneNumber);
            parameters.Add("UserType", userDto.UserType);
            parameters.Add("Location", userDto.Location);
            parameters.Add("CreatedAt", DateTime.UtcNow);
            parameters.Add("UpdatedAt", DateTime.UtcNow);

            var insertQuery = @"
        INSERT INTO Users (FirstName, LastName, Email, PasswordHash, CompanyId,
                           PhoneNumber, UserType, Location, CreatedAt, UpdatedAt)
        VALUES (@FirstName, @LastName, @Email, @PasswordHash, @CompanyId,
                @PhoneNumber, @UserType, @Location, @CreatedAt, @UpdatedAt);
        SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var newUserId = await connection.ExecuteScalarAsync<int>(insertQuery, parameters);
            return new ResponseDto
            {
                IsSuccess = newUserId > 0,
                Message = "User inserted successfully.",
                Id = newUserId
            };
        }
        else
        {
            // For updates, only update fields that are not null/empty
            var setClauses = new List<string>();

            if (!string.IsNullOrEmpty(userDto.FirstName))
            {
                setClauses.Add("FirstName = @FirstName");
                parameters.Add("FirstName", userDto.FirstName);
            }

            if (!string.IsNullOrEmpty(userDto.LastName))
            {
                setClauses.Add("LastName = @LastName");
                parameters.Add("LastName", userDto.LastName);
            }

            if (!string.IsNullOrEmpty(userDto.Email))
            {
                setClauses.Add("Email = @Email");
                parameters.Add("Email", userDto.Email);
            }

            if (!string.IsNullOrEmpty(userDto.PasswordHash))
            {
                setClauses.Add("PasswordHash = @PasswordHash");
                parameters.Add("PasswordHash", userDto.PasswordHash); // Already hashed
            }

            if (userDto.CompanyId.HasValue)
            {
                setClauses.Add("CompanyId = @CompanyId");
                parameters.Add("CompanyId", userDto.CompanyId);
            }

            if (!string.IsNullOrEmpty(userDto.PhoneNumber))
            {
                setClauses.Add("PhoneNumber = @PhoneNumber");
                parameters.Add("PhoneNumber", userDto.PhoneNumber);
            }

            if (userDto.UserType > 0)  // Assuming 0 is not a valid UserType
            {
                setClauses.Add("UserType = @UserType");
                parameters.Add("UserType", userDto.UserType);
            }

            if (!string.IsNullOrEmpty(userDto.Location))
            {
                setClauses.Add("Location = @Location");
                parameters.Add("Location", userDto.Location);
            }

            if (!string.IsNullOrEmpty(userDto.ProfilePicture))
            {
                setClauses.Add("ProfilePicture = @ProfilePicture");
                parameters.Add("ProfilePicture", userDto.ProfilePicture);
            }

            if (!string.IsNullOrEmpty(userDto.LinkedinProfile))
            {
                setClauses.Add("LinkedinProfile = @LinkedinProfile");
                parameters.Add("LinkedinProfile", userDto.LinkedinProfile);
            }

            if (!string.IsNullOrEmpty(userDto.Bio))
            {
                setClauses.Add("Bio = @Bio");
                parameters.Add("Bio", userDto.Bio);
            }

            if (userDto.DateOfBirth.HasValue)
            {
                setClauses.Add("DateOfBirth = @DateOfBirth");
                parameters.Add("DateOfBirth", userDto.DateOfBirth);
            }
            if (!string.IsNullOrEmpty(userDto.Skills))
            {
                setClauses.Add("Skills = @Skills");
                parameters.Add("Skills", userDto.Skills);
            }
            if (!string.IsNullOrEmpty(userDto.Education))
            {
                setClauses.Add("Education = @Education");
                parameters.Add("Education", userDto.Education);

            }

            if (!string.IsNullOrEmpty(userDto.Experiences))
            {
                setClauses.Add("Experiences = @Experiences"); 
                parameters.Add("Experiences", userDto.Experiences);
            }

            // Always update the UpdatedAt field
            setClauses.Add("UpdatedAt = @UpdatedAt");
            parameters.Add("UpdatedAt", DateTime.UtcNow);
            parameters.Add("UserId", userDto.UserId);

            if (!setClauses.Any())
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "No fields to update.",
                    Id = userDto.UserId
                };
            }

            var updateQuery = $@"
                            UPDATE Users
                            SET {string.Join(", ", setClauses)}
                            WHERE UserId = @UserId";

            var rowsAffected = await connection.ExecuteAsync(updateQuery, parameters);
            return new ResponseDto
            {
                IsSuccess = rowsAffected > 0,
                Message = rowsAffected > 0 ? "User updated successfully." : "Failed to update user.",
                Id = rowsAffected > 0 ? userDto.UserId : -1
            };
        }
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
                         FirstName,
                         LastName,
                         Bio,
                         Location,
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
            SELECT *
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

    public async Task<ResponseDto> UploadUserProfilePictureAsync(int userId, string? imageUrl, string? bio)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = @"
        UPDATE Users
        SET 
            ProfilePicture = @ProfilePicture,
            Bio = @Bio
        WHERE UserId = @UserId";

        var parameters = new DynamicParameters();
        parameters.Add("@ProfilePicture", imageUrl ?? (object)DBNull.Value);
        parameters.Add("@Bio", bio ?? (object)DBNull.Value);
        parameters.Add("@UserId", userId);

        var rows = await connection.ExecuteAsync(sql, parameters);
        return new ResponseDto
        {
            IsSuccess = rows > 0,
            Message = rows > 0 ? "Profile updated." : "User not found."
        };
    }

    public async Task<UsersProfileDto?> GetUploadedProfileByIdAsync(int userId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = @"SELECT * FROM Users WHERE UserId = @UserId";

        var user = await connection.QueryFirstOrDefaultAsync<UsersProfileDto>(query, new { UserId = userId });

        return user;
    }
}