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
                      CreatedAt,
                      LinkUrl 
               FROM Notifications
               """;

        return await connection.QueryAsync<NotificationsDataModel>(sql).ConfigureAwait(false);
    }

    public async Task<NotificationsDataModel?> GetNotificationsByIdAsync(Guid notificationsId) // Changed parameter to Guid
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
                      CreatedAt,
                      LinkUrl 
               FROM Notifications
               WHERE NotificationId = @NotificationId
               """;

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", notificationsId, DbType.Guid); // Changed DbType to Guid

        // Use QueryFirstOrDefaultAsync for single results, as it handles null if not found
        return await connection.QueryFirstOrDefaultAsync<NotificationsDataModel>(sql, parameters).ConfigureAwait(false);
    }

    public async Task<ResponseDto> SubmitNotificationsAsync(NotificationsDataModel notificationsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;
        int affectedRows;

        var parameters = new DynamicParameters();

        // Check if NotificationId is empty, meaning new record
        if (notificationsDto.NotificationId == Guid.Empty)
        {
            // Generate new GUID client-side
            notificationsDto.NotificationId = Guid.NewGuid();

            sql = """
        INSERT INTO Notifications (NotificationId, UserId, Title, Message, NotificationTypeId, IsRead, CreatedAt, LinkUrl)
        VALUES (@NotificationId, @UserId, @Title, @Message, @NotificationTypeId, @IsRead, @CreatedAt, @LinkUrl);
        """;

            parameters.Add("@NotificationId", notificationsDto.NotificationId, DbType.Guid);
        }
        else
        {
            sql = """
        UPDATE Notifications
        SET
            UserId = @UserId,
            Title = @Title,
            Message = @Message,
            NotificationTypeId = @NotificationTypeId,
            IsRead = @IsRead,
            CreatedAt = @CreatedAt,
            LinkUrl = @LinkUrl
        WHERE NotificationId = @NotificationId
        """;

            parameters.Add("@NotificationId", notificationsDto.NotificationId, DbType.Guid);
        }

        parameters.Add("@UserId", notificationsDto.UserId, DbType.Int32);
        parameters.Add("@Title", notificationsDto.Title, DbType.String);
        parameters.Add("@Message", notificationsDto.Message, DbType.String);
        parameters.Add("@NotificationTypeId", notificationsDto.NotificationTypeId, DbType.Int32);
        parameters.Add("@IsRead", notificationsDto.IsRead, DbType.Boolean);
        parameters.Add("@CreatedAt", notificationsDto.CreatedAt, DbType.DateTime2);
        parameters.Add("@LinkUrl", notificationsDto.LinkUrl, DbType.String);

        try
        {
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

            return new ResponseDto
            {
                IsSuccess = affectedRows > 0,
                Message = affectedRows > 0 ? "Notification saved successfully." : "Failed to save notification.",
                Guid Id = notificationsDto.NotificationId 
            };

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database error in SubmitNotificationsAsync: {ex}");
            return new ResponseDto
            {
                IsSuccess = false,
                Message = $"Database error: {ex.Message}"
            };
        }
    }


    public async Task<ResponseDto> DeleteNotificationsAsync(Guid notificationsId) // Changed parameter to Guid
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete a notification by ID
        var sql = """DELETE FROM Notifications WHERE NotificationId = @NotificationId""";

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", notificationsId, DbType.Guid); // Changed DbType to Guid

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