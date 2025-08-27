using Dapper;
using JobApplicationTracker.Data.Dto.Requests;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using System.Data;

namespace JobApplicationTracker.Data.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDatabaseConnectionService _connectionService;

        public NotificationRepository(IDatabaseConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = @"
                    INSERT INTO notifications 
                        (UserId, NotificationTypeId, JobId, Title, Message, LinkUrl, IsRead, CreatedAt)
                    VALUES 
                        (@UserId, @NotificationTypeId, @JobId, @Title, @Message, @LinkUrl, 0, @CreatedAt);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", dto.UserId, DbType.Int32);
                parameters.Add("@NotificationTypeId", dto.NotificationTypeId, DbType.Int32);
                parameters.Add("@JobId", dto.JobId, DbType.Int32);
                parameters.Add("@Title", dto.Title, DbType.String);
                parameters.Add("@Message", dto.Message, DbType.String);
                parameters.Add("@LinkUrl", dto.LinkUrl, DbType.String);
                parameters.Add("@CreatedAt", DateTime.Now, DbType.DateTime);

                var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);

                return new NotificationDto
                {
                    NotificationId = newId,
                    UserId = dto.UserId,
                    NotificationTypeId = dto.NotificationTypeId,
                    JobId = dto.JobId,
                    Title = dto.Title,
                    Message = dto.Message,
                    LinkUrl = dto.LinkUrl,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating notification: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = @"
                    SELECT NotificationId, UserId, NotificationTypeId, JobId, Title, Message, 
                           IsRead, CreatedAt, LinkUrl
                    FROM notifications
                    WHERE UserId = @UserId
                    ORDER BY CreatedAt DESC";

                return await connection.QueryAsync<NotificationDto>(sql, new { UserId = userId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving user notifications: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = @"
                    SELECT NotificationId, UserId, NotificationTypeId, JobId, Title, Message, 
                           IsRead, CreatedAt, LinkUrl
                    FROM notifications
                    WHERE UserId = @UserId AND (IsRead = 0 OR IsRead IS NULL)
                    ORDER BY CreatedAt DESC";

                return await connection.QueryAsync<NotificationDto>(sql, new { UserId = userId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving unread notifications: {ex.Message}", ex);
            }
        }

        public async Task<ResponseDto> MarkAsReadAsync(int notificationId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = "UPDATE notifications SET IsRead = 1 WHERE NotificationId = @NotificationId";

                var affectedRows = await connection.ExecuteAsync(sql, new { NotificationId = notificationId }).ConfigureAwait(false);

                return new ResponseDto
                {
                    IsSuccess = affectedRows > 0,
                    Message = affectedRows > 0 ? "Notification marked as read." : "Notification not found.",
                    Id = notificationId
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error marking notification as read: {ex.Message}",
                    Id = notificationId
                };
            }
        }

        public async Task<ResponseDto> MarkAllAsReadAsync(int userId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = "UPDATE notifications SET IsRead = 1 WHERE UserId = @UserId AND (IsRead = 0 OR IsRead IS NULL)";

                var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId }).ConfigureAwait(false);

                return new ResponseDto
                {
                    IsSuccess = affectedRows > 0,
                    Message = $"{affectedRows} notifications marked as read.",
                    Id = userId
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error marking all notifications as read: {ex.Message}",
                    Id = userId
                };
            }
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(int notificationId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = @"
                    SELECT NotificationId, UserId, NotificationTypeId, JobId, Title, Message, 
                           IsRead, CreatedAt, LinkUrl
                    FROM notifications
                    WHERE NotificationId = @NotificationId";

                return await connection.QueryFirstOrDefaultAsync<NotificationDto>(
                    sql, new { NotificationId = notificationId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving notification by ID: {ex.Message}", ex);
            }
        }

        public async Task<ResponseDto> DeleteNotificationAsync(int notificationId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = "DELETE FROM notifications WHERE NotificationId = @NotificationId";

                var affectedRows = await connection.ExecuteAsync(sql, new { NotificationId = notificationId }).ConfigureAwait(false);

                return new ResponseDto
                {
                    IsSuccess = affectedRows > 0,
                    Message = affectedRows > 0 ? "Notification deleted successfully." : "Notification not found.",
                    Id = notificationId
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error deleting notification: {ex.Message}",
                    Id = notificationId
                };
            }
        }

        public async Task<IEnumerable<UsersDtoResponse>> GetUsersBySkillsAsync(string jobSkills)
        {
            try
            {
                if (string.IsNullOrEmpty(jobSkills))
                    return Enumerable.Empty<UsersDtoResponse>();

                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                // Split job skills by comma and clean them
                var skillsList = jobSkills.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(s => s.Trim())
                                         .Where(s => !string.IsNullOrEmpty(s))
                                         .ToList();

                if (!skillsList.Any())
                    return Enumerable.Empty<UsersDtoResponse>();

                // Create LIKE conditions for each skill
                var conditions = skillsList.Select((skill, index) => $"Skills LIKE @skill{index}").ToList();
                var whereClause = string.Join(" OR ", conditions);

                var sql = $@"
                    SELECT UserId, FirstName, LastName, Email, UserType, PhoneNumber, 
                           CreatedAt, UpdatedAt, ProfilePicture, ResumeUrl, PortfolioUrl, 
                           LinkedinProfile, Location, Headline, Bio, Skills, Education, Experiences
                    FROM users
                    WHERE ({whereClause}) AND Email IS NOT NULL AND Email != ''
                    ORDER BY FirstName, LastName";

                var parameters = new DynamicParameters();
                for (int i = 0; i < skillsList.Count; i++)
                {
                    parameters.Add($"@skill{i}", $"%{skillsList[i]}%", DbType.String);
                }

                var users = await connection.QueryAsync<UsersDtoResponse>(sql, parameters).ConfigureAwait(false);

                // Remove duplicates based on UserId
                return users.GroupBy(u => u.UserId).Select(g => g.First()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving users by skills: {ex.Message}", ex);
            }
        }

        public async Task<JobDto?> GetJobByIdAsync(int jobId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = @"
                    SELECT JobId, PostedByUserId, JobType, Description, Requirements, Location, 
                           EmpolymentType, IsPublished, SalaryRangeMin, SalaryRangeMax, 
                           ExperienceLevel, Status, PostedAt, ApplicationDeadline, Skills
                    FROM job
                    WHERE JobId = @JobId";

                return await connection.QueryFirstOrDefaultAsync<JobDto>(sql, new { JobId = jobId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving job by ID: {ex.Message}", ex);
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                await using var connection = await _connectionService.GetDatabaseConnectionAsync();

                var sql = "SELECT COUNT(*) FROM notifications WHERE UserId = @UserId AND (IsRead = 0 OR IsRead IS NULL)";

                return await connection.QuerySingleAsync<int>(sql, new { UserId = userId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting unread count: {ex.Message}", ex);
            }
        }
    }
}