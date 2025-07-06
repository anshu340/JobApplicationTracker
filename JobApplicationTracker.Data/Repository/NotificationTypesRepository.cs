using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Api.Data.Service;

public class NotificationTypesRepository : INotificationsTypesRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public NotificationTypesRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<NotificationTypesDataModel>> GetAllNotificationTypesAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT NotificationTypeId, 
                         TypeName, 
                         Description 
                         Priority
                  FROM NotificationTypes
                  """;

        return await connection.QueryAsync< NotificationTypesDataModel >(sql).ConfigureAwait(false);
    }

    public async Task<NotificationTypesDataModel> GetNotificationTypesByIdAsync(int notificationTypeId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to fetch a notification type by ID
        var sql = """
              SELECT NotificationTypeId, 
                     TypeName, 
                     Description, 
                     Priority
              FROM NotificationTypes
              WHERE NotificationTypeId = @NotificationTypeId
              """;

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationTypeId", notificationTypeId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<NotificationTypesDataModel>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitNotificationTypesAsync(NotificationTypesDataModel notificationTypesDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (notificationTypesDto.NotificationTypeId <= 0)
        {
            // Insert new notification type (assumes NotificationTypeId is auto-incremented)
            sql = """
        INSERT INTO NotificationTypes (TypeName, Description, Priority)
        VALUES (@TypeName, @Description, @Priority);
        """;
        }
        else
        {
            // Update existing notification type
            sql = """
        UPDATE NotificationTypes
        SET 
            TypeName = @TypeName,
            Description = @Description,
            Priority = @Priority
        WHERE NotificationTypeId = @NotificationTypeId;
        """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationTypeId", notificationTypesDto.NotificationTypeId, DbType.Int32);
        parameters.Add("@TypeName", notificationTypesDto.TypeName, DbType.String);
        parameters.Add("@Description", notificationTypesDto.Description, DbType.String);
        parameters.Add("@Priority", notificationTypesDto.Priority, DbType.Int32); // Assuming Priority is an integer

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Notifications type submitted successfully." : "Failed to submit notification type."
        };
    }
    public async Task<ResponseDto> DeleteNotificationTypesAsync(int notificationTypesId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // Check referential integrity 
        var refCheckSql = "SELECT COUNT(1) FROM SomeRelatedTable WHERE NotificationTypeId = @NotificationTypeId"; // Adjust table name as needed
        var hasDependencies = await connection.ExecuteScalarAsync<int>(refCheckSql, new { NotificationTypeId = notificationTypesId });

        if (hasDependencies > 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Cannot delete notification type because it has associated records."
            };
        }

        // Delete the notification type
        var deleteSql = "DELETE FROM NotificationTypes WHERE NotificationTypeId = @NotificationTypeId";

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationTypeId", notificationTypesId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(deleteSql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Notifications type not found or could not be deleted."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Notifications type deleted successfully."
        };
    }
}
