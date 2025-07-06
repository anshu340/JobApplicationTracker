using Dapper;
using JobApplicationTracker.Data.Interface;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Repository;

public class NotificationsRepository : INotificationsRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public NotificationsRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<NotificationsDataModel>> GetAllNotificationsAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
              SELECT NotificationId, 
                     UserId, 
                     Title, 
                     Message, 
                     NotificationTypeId,    
                     IsRead, 
                     CreatedAt 
              FROM Notifications
              """;

        return await connection.QueryAsync<NotificationsDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<NotificationsDataModel> GetNotificationsByIdAsync(int notificationsId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to fetch a notification by ID
        var sql = """
              SELECT NotificationId, 
                     UserId, 
                     Title, 
                     Message, 
                     NotificationTypeId, 
                     IsRead, 
                     CreatedAt 
              FROM Notifications
              WHERE NotificationId = @NotificationId
              """;

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", notificationsId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<NotificationsDataModel>(sql, parameters).ConfigureAwait(false);
    }
    public async Task<ResponseDto> SubmitNotificationsAsync(NotificationsDataModel notificationsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (notificationsDto.NotificationId <= 0)
        {
            // Insert new notification
            sql = """
        INSERT INTO Notifications (UserId, Title, Message, NotificationTypeId, IsRead, CreatedAt)
        VALUES (@UserId, @Title, @Message, @NotificationTypeId, @IsRead, @CreatedAt);
        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;
        }
        else
        {
            // Update existing notification
            sql = """
        UPDATE Notifications
        SET 
            UserId = @UserId,
            Title = @Title,
            Message = @Message,
            NotificationTypeId = @NotificationTypeId,
            IsRead = @IsRead,
            CreatedAt = @CreatedAt
        WHERE NotificationId = @NotificationId
        """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", notificationsDto.NotificationId, DbType.Int32);
        parameters.Add("@UserId", notificationsDto.UserId, DbType.Int32);
        parameters.Add("@Title", notificationsDto.Title, DbType.String);
        parameters.Add("@Message", notificationsDto.Message, DbType.String);
        parameters.Add("@NotificationTypeId", notificationsDto.NotificationTypeId, DbType.Int32);
        parameters.Add("@IsRead", notificationsDto.IsRead, DbType.Boolean);
        parameters.Add("@CreatedAt", notificationsDto.CreatedAt, DbType.DateTime);

        var affectedRows = 0;

        if (notificationsDto.NotificationId <= 0)
        {
            // Insert operation
            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            notificationsDto.NotificationId = newId; // Set the ID for the newly inserted record
        }
        else
        {
            // Update operation
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Notifications submitted successfully." : "Failed to submit notification."
        };
    }



    public async Task<ResponseDto> DeleteNotificationsAsync(int notificationsId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a notification by ID
        var sql = """DELETE FROM Notifications WHERE NotificationId = @NotificationId""";

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", notificationsId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete notification."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Notifications deleted successfully."
        };
    }
}