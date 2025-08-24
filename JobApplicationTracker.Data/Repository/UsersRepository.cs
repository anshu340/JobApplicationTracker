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
                  SELECT UserId, FirstName, LastName, Email, UserType, PhoneNumber, CreatedAt, UpdatedAt,
                         ProfilePicture, ResumeUrl, PortfolioUrl, LinkedinProfile, Location, Headline, Bio,
                         Skills, Education, Experiences
                  FROM Users where (CompanyId =@companyId or @companyId = 0)
                  """;
        var parameters = new DynamicParameters();
        parameters.Add("@companyId", companyId, DbType.Int32);

        var users = await connection.QueryAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);

        // Populate ExperienceList for each user
        foreach (var user in users)
        {
            user.ExperienceList = await GetUserExperiencesByUserIdInternalAsync(user.UserId);
        }

        return users;
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
            Education = user.Education
        };

        // Load experiences - if Experiences property is string type, keep the comma-separated IDs
        // If you want the experience objects, add a separate property like ExperienceList
        // For now, keeping the original experiences string from database
        profile.Experiences = user.Experiences; // This assigns the comma-separated string

        // If UsersProfileDto has an ExperienceList property, uncomment the line below:
        // profile.ExperienceList = await GetUserExperiencesByUserIdInternalAsync(userId);

        // Add company profile with CompanyLogo if CompanyId > 0
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
                    CompanyLogo = company.CompanyLogo,
                    ContactEmail = company.ContactEmail
                };
            }
        }

        return profile;
    }

    // Method to get experiences by userId (using "Experiences" column instead of ExperienceIds)
    private async Task<List<ExperienceDto>> GetUserExperiencesByUserIdInternalAsync(int userId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        // First get the Experiences from Users table
        var experienceIds = await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT Experiences FROM Users WHERE UserId = @UserId",
            new { UserId = userId });

        if (string.IsNullOrEmpty(experienceIds))
            return new List<ExperienceDto>();

        // Parse comma-separated IDs
        var ids = experienceIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                              .Where(id => id > 0)
                              .ToList();

        if (!ids.Any())
            return new List<ExperienceDto>();

        var sql = $"""
            SELECT ExperienceId, JobTitle, Organization, Location, StartMonth, StartYear,
                   EndMonth, EndYear, Description, IsCurrentlyWorking
            FROM Experiences 
            WHERE ExperienceId IN ({string.Join(",", ids)})
            ORDER BY StartYear DESC, StartMonth DESC
        """;

        var experiences = await connection.QueryAsync<ExperienceDto>(sql);
        return experiences.ToList();
    }

    public async Task<UsersDtoResponse?> GetUsersByIdAsync(int usersId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
                SELECT UserId, FirstName, LastName, Email, UserType, PhoneNumber, CreatedAt, UpdatedAt,
                       ProfilePicture, ResumeUrl, PortfolioUrl, LinkedinProfile, Location, Headline, Bio,
                       Skills, Education, Experiences
                FROM Users 
                WHERE UserId = @UserId
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", usersId, DbType.Int32);

        var user = await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);

        if (user != null)
        {
            user.ExperienceList = await GetUserExperiencesByUserIdInternalAsync(user.UserId);
        }

        return user;
    }

    public async Task<UsersDtoResponse?> GetUserByEmail(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT UserId, FirstName, LastName, Email, UserType, PhoneNumber, CreatedAt, UpdatedAt,
                         ProfilePicture, ResumeUrl, PortfolioUrl, LinkedinProfile, Location, Headline, Bio,
                         Skills, Education, Experiences
                  FROM Users 
                  WHERE Email = @Email
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        var user = await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);

        if (user != null)
        {
            user.ExperienceList = await GetUserExperiencesByUserIdInternalAsync(user.UserId);
        }

        return user;
    }

    public async Task<UsersDtoResponse?> GetUserByPhone(string phone)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT UserId, FirstName, LastName, Email, UserType, PhoneNumber, CreatedAt, UpdatedAt,
                         ProfilePicture, ResumeUrl, PortfolioUrl, LinkedinProfile, Location, Headline, Bio,
                         Skills, Education, Experiences
                  FROM Users 
                  WHERE PhoneNumber = @PhoneNumber
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@PhoneNumber", phone, DbType.String);

        var user = await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);

        if (user != null)
        {
            user.ExperienceList = await GetUserExperiencesByUserIdInternalAsync(user.UserId);
        }

        return user;
    }

    public async Task<UsersDataModel?> GetUserForLoginAsync(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT UserId, FirstName, LastName, Email, PasswordHash, UserType, PhoneNumber, 
                         CreatedAt, UpdatedAt, ProfilePicture, ResumeUrl, PortfolioUrl, LinkedinProfile, 
                         Location, Headline, Bio, Skills, Education, Experiences, CompanyId, DateOfBirth
                  FROM Users 
                  WHERE Email = @Email
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        var user = await connection.QueryFirstOrDefaultAsync<UsersDataModel>(sql, parameters).ConfigureAwait(false);
        return user;
    }

    public async Task<bool> DoesEmailExists(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        var count = await connection.ExecuteScalarAsync<int>(sql, parameters).ConfigureAwait(false);
        return count > 0;
    }

    public async Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();
        var isNewUser = userDto.UserId <= 0;
        var parameters = new DynamicParameters();

        if (isNewUser)
        {
            parameters.Add("FirstName", userDto.FirstName ?? "");
            parameters.Add("LastName", userDto.LastName ?? "");
            parameters.Add("Email", userDto.Email ?? "");
            parameters.Add("PasswordHash", userDto.PasswordHash ?? "");
            parameters.Add("CompanyId", userDto.CompanyId);
            parameters.Add("PhoneNumber", userDto.PhoneNumber);
            parameters.Add("UserType", userDto.UserType);
            parameters.Add("Location", userDto.Location);
            parameters.Add("Experiences", userDto.Experiences); // Changed
            parameters.Add("CreatedAt", DateTime.UtcNow);
            parameters.Add("UpdatedAt", DateTime.UtcNow);

            var insertQuery = @"
        INSERT INTO Users (FirstName, LastName, Email, PasswordHash, CompanyId,
                           PhoneNumber, UserType, Location, Experiences, CreatedAt, UpdatedAt)
        VALUES (@FirstName, @LastName, @Email, @PasswordHash, @CompanyId,
                @PhoneNumber, @UserType, @Location, @Experiences, @CreatedAt, @UpdatedAt);
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
                parameters.Add("PasswordHash", userDto.PasswordHash);
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
            if (userDto.UserType > 0)
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
            if (!string.IsNullOrEmpty(userDto.Experiences)) // Changed
            {
                setClauses.Add("Experiences = @Experiences");
                parameters.Add("Experiences", userDto.Experiences);
            }

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

        var rowsAffected = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "User deleted successfully." : "Failed to delete user.",
            Id = rowsAffected > 0 ? userId : -1
        };
    }

    public async Task<ResponseDto> UploadUserProfilePictureAsync(int userId, string? imageUrl, string? bio)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var setClauses = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(imageUrl))
        {
            setClauses.Add("ProfilePicture = @ProfilePicture");
            parameters.Add("ProfilePicture", imageUrl);
        }

        if (!string.IsNullOrEmpty(bio))
        {
            setClauses.Add("Bio = @Bio");
            parameters.Add("Bio", bio);
        }

        if (!setClauses.Any())
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "No data to update.",
                Id = userId
            };
        }

        setClauses.Add("UpdatedAt = @UpdatedAt");
        parameters.Add("UpdatedAt", DateTime.UtcNow);
        parameters.Add("UserId", userId);

        var updateQuery = $"""
                          UPDATE Users
                          SET {string.Join(", ", setClauses)}
                          WHERE UserId = @UserId
                          """;

        var rowsAffected = await connection.ExecuteAsync(updateQuery, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "Profile updated successfully." : "Failed to update profile.",
            Id = rowsAffected > 0 ? userId : -1
        };
    }

    public async Task<UsersProfileDto?> GetUploadedProfileByIdAsync(int id)
    {
        // This method appears to be the same as GetUserProfileAsync based on the naming
        // You might want to clarify the difference or simply call the existing method
        return await GetUserProfileAsync(id);
    }

    // Method to add experience ID to user (using "Experiences")
    public async Task<ResponseDto> AddExperienceToUserAsync(int userId, int experienceId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var currentExperienceIds = await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT Experiences FROM Users WHERE UserId = @UserId",
            new { UserId = userId });

        var experienceIdsList = new List<int>();

        if (!string.IsNullOrEmpty(currentExperienceIds))
        {
            experienceIdsList = currentExperienceIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                                                   .Where(id => id > 0)
                                                   .ToList();
        }

        if (!experienceIdsList.Contains(experienceId))
        {
            experienceIdsList.Add(experienceId);
        }

        var newExperienceIds = string.Join(",", experienceIdsList);
        var rowsAffected = await connection.ExecuteAsync(
            "UPDATE Users SET Experiences = @Experiences, UpdatedAt = @UpdatedAt WHERE UserId = @UserId",
            new { Experiences = newExperienceIds, UpdatedAt = DateTime.UtcNow, UserId = userId });

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "Experience added to user successfully." : "Failed to add experience."
        };
    }

    // Method to remove experience ID from user (using "Experiences")
    public async Task<ResponseDto> RemoveExperienceFromUserAsync(int userId, int experienceId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var currentExperienceIds = await connection.QueryFirstOrDefaultAsync<string>(
            "SELECT Experiences FROM Users WHERE UserId = @UserId",
            new { UserId = userId });

        if (string.IsNullOrEmpty(currentExperienceIds))
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "User has no experiences to remove."
            };
        }

        var experienceIdsList = currentExperienceIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                                   .Select(id => int.TryParse(id.Trim(), out var parsed) ? parsed : 0)
                                                   .Where(id => id > 0 && id != experienceId)
                                                   .ToList();

        var newExperienceIds = experienceIdsList.Any() ? string.Join(",", experienceIdsList) : null;
        var rowsAffected = await connection.ExecuteAsync(
            "UPDATE Users SET Experiences = @Experiences, UpdatedAt = @UpdatedAt WHERE UserId = @UserId",
            new { Experiences = newExperienceIds, UpdatedAt = DateTime.UtcNow, UserId = userId });

        return new ResponseDto
        {
            IsSuccess = rowsAffected > 0,
            Message = rowsAffected > 0 ? "Experience removed from user successfully." : "Failed to remove experience."
        };
    }

    // Public method required by interface - returns UsersProfileDto as per interface
    public async Task<UsersProfileDto?> GetUserExperiencesByUserIdAsync(int userId)
    {
        return await GetUserProfileAsync(userId);
    }
}