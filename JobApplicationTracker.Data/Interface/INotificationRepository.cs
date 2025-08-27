using JobApplicationTracker.Data.Dto.Requests;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface
{
    public interface INotificationRepository
    {
        // Create notification
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);

        // Get user notifications
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);

        // Mark as read
        Task<ResponseDto> MarkAsReadAsync(int notificationId);
        Task<ResponseDto> MarkAllAsReadAsync(int userId);

        // Get notification by ID
        Task<NotificationDto?> GetNotificationByIdAsync(int notificationId);

        // Delete notification
        Task<ResponseDto> DeleteNotificationAsync(int notificationId);

        // Get users by skills for job matching
        Task<IEnumerable<UsersDtoResponse>> GetUsersBySkillsAsync(string jobSkills);

        // Get job details by ID
        Task<JobDto?> GetJobByIdAsync(int jobId);

        // Get unread count
        Task<int> GetUnreadCountAsync(int userId);
    }
}