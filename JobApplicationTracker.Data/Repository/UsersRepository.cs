using Dapper;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.AuthDto;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;

namespace JobApplicationTracker.Data.Repository;

public class UsersRepository(IDatabaseConnectionService connectionService) : IUserRepository
{
    public async Task<IEnumerable<UsersDtoResponse>> GetAllUsersAsync()
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT UserId, 
                         Email, 
                         UserType,
                         PhoneNumber,
                         CreatedAt,
                         UpdatedAt,
                  FROM Users
                  """; 

        return await connection.QueryAsync<UsersDtoResponse>(sql).ConfigureAwait(false);
    }

    public async Task<UsersDtoResponse?> GetUsersByIdAsync(int usersId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();
        // write the SQL query to fetch a Users by ID
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
        parameters.Add("@UsersId", usersId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        string sql = 
            
            userDto.UserId <= 0 ?   
            
            @"INSERT INTO Users (Email, PasswordHash, UserType, CreatedAt, UpdatedAt) 
                                     VALUES (@Email, @PasswordHash, @UserType, @CreatedAt, @UpdatedAt)"
            :   @" UPDATE Users
                      SET Email = @Email,
                          PasswordHash = @PasswordHash,
                          UserType = @UserType,
                          UpdatedAt = @UpdatedAt,
                      WHERE UserId = @UserId
                      ";

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userDto.UserId, DbType.Int32);
        parameters.Add("Email", userDto.Email, DbType.String);
        parameters.Add("PasswordHash", userDto.PasswordHash, DbType.String);
        parameters.Add("UserType", userDto.UserType, DbType.String);
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
        var query = @"INSERT INTO Users (Email, PasswordHash, UserType, CreatedAt, UpdatedAt) 
                                   VALUES (@Email, @PasswordHash, @UserType, @CreatedAt, @UpdatedAt);
                    SELECT SCOPE_IDENTITY();";
        
        var parameters = new DynamicParameters();
        parameters.Add("Email", userDto.Email, DbType.String);
        parameters.Add("PasswordHash", userDto.PasswordHash, DbType.String);
        parameters.Add("UserType", userDto.UserType, DbType.String);
        parameters.Add("CreatedAt", DateTime.UtcNow, DbType.DateTime);
        parameters.Add("UpdatedAt", DateTime.UtcNow, DbType.DateTime);
        
        
        return await connection.ExecuteScalarAsync<int>(query, parameters).ConfigureAwait(false);
    }
    
    public async Task<ResponseDto> DeleteUsersAsync(int userId)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        // Write the SQL query to delete a user by ID
        var sql = "DELETE FROM Users WHERE UserId = @UserId";

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", userId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete user."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "User deleted successfully."
        };
    }
    
    public async Task<bool> DoesEmailExists(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = @"SELECT 1 FROM Users WHERE LOWER(Email) = LOWER(@Email)";

        var parameters = new DynamicParameters();
        parameters.Add("@Email",email, DbType.String);

        var result = await connection.ExecuteScalarAsync<int?>(query,parameters).ConfigureAwait(false);

        return result.HasValue;
    }

    public async Task<UsersDtoResponse?> GetUserByPhone(string phone)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = @"SELECT UserId,
                             Email,
                             UserType,
                             PhoneNumber,
                             CreatedAt,
                             UpdatedAt 
                              FROM Users WHERE PhoneNumber = @PhoneNumber";
        var parameters = new DynamicParameters();
        parameters.Add("@PhoneNumber", phone, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(query,parameters).ConfigureAwait(false);
    }

    public async Task<UsersDtoResponse?> GetUserByEmail(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();

        var query = @"SELECT 
                          UserId,
                         Email,
                         UserType,
                         PhoneNumber,
                         CreatedAt,
                         UpdatedAt
                       FROM Users WHERE Email = @Email";

        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UsersDtoResponse>(query,parameters).ConfigureAwait(false);
    }
    
    public async Task<UsersDataModel?> GetUserForLoginAsync(string email)
    {
        await using var connection = await connectionService.GetDatabaseConnectionAsync();
        var query = @"SELECT UserId, Email, PasswordHash, UserType FROM Users WHERE Email = @Email";

        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);
        
        return await connection.QueryFirstOrDefaultAsync<UsersDataModel>(query, parameters).ConfigureAwait(false);
    }
}
   

