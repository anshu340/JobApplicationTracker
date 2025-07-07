using Dapper;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.AuthDto;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;

namespace JobApplicationTracker.Data.Repository;

public class UsersRepository : IUserRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public UsersRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<UsersDataModel>> GetAllUsersAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT UserId, 
                         Email, 
                         PasswordHash
                         UserTypeId
                         IsAdmin
                         CreatedAt
                         UpdatedAt
                         IsActive
                  FROM Users
                  """; 

        return await connection.QueryAsync<UsersDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<UsersDataModel> GetUsersByIdAsync(int usersId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        // write the SQL query to fetch a Users by ID
        var sql = """
                    SELECT * 
                    FROM Users 
                    WHERE UserId = @UserId
                    """;

        var parameters = new DynamicParameters();
        parameters.Add("@UsersId", usersId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<UsersDataModel>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitUsersAsync(UsersDataModel userDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

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
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();
        var query = @"INSERT INTO Users (Email, PasswordHash, UserType, CreatedAt, UpdatedAt) 
                                   VALUES (@Email, @PasswordHash, @UserType, @CreatedAt, @UpdatedAt)";
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
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

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

    public async Task<ResponseDto> CreateUserAsync(SignUpDto credentials)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var query = 
            @" 
                INSERT INTO Users (Email, PasswordHash, UserTypeId, IsAdmin, CreatedAt, UpdatedAt, IsActive)
                VALUES(@Email, @PasswordHash, @UserTypeId, @IsAdmin, @CreatedAt, @UpdatedAt, @IsActive)
            ";


        var parameters = new DynamicParameters();
        parameters.Add("Email", credentials.Email, DbType.String);
        parameters.Add("PasswordHash", credentials.PasswordHash, DbType.String);

        var affectedRows = await connection.ExecuteAsync(query, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to create user."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "User created successfully."
        };
    }

    public async Task<bool> DoesEmailExists(string email)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var query = """SELECT 1 FROM Users WHERE LOWER(Email) = LOWER(@Email)""";

        var parameters = new DynamicParameters();
        parameters.Add("@Email",email, DbType.String);

        var result = await connection.ExecuteScalarAsync<int?>(query,parameters).ConfigureAwait(false);

        return result.HasValue;
    }

    public async Task<UsersDataModel?> GetUserByPhone(string phone)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var query = """SELECT * FROM Users WHERE PhoneNumber = @PhoneNumber""";

        var parameters = new DynamicParameters();
        parameters.Add("@PhoneNumber", phone, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UsersDataModel>(query,parameters).ConfigureAwait(false);
    }

    public async Task<UsersDataModel?> GetUserByEmail(string email)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var query = """SELECT * FROM Users WHERE Email = @Email""";

        var parameters = new DynamicParameters();
        parameters.Add("@Email", email, DbType.String);

        return await connection.QueryFirstOrDefaultAsync<UsersDataModel>(query,parameters).ConfigureAwait(false);
    }
}
   

