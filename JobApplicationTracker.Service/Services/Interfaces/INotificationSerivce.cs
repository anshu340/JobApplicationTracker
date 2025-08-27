using JobApplicationTracker.Data.Dto.Requests;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Business.Interface
{
    public interface INotificationService
    {
        // Main skill-based notification method
        Task<NotificationResponseDto> SendJobSkillNotificationsAsync(JobSkillNotificationDto dto);

        // User notification methods
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId);
        Task<int> GetUnreadCountAsync(int userId);

        // Mark as read methods
        Task<ResponseDto> MarkAsReadAsync(MarkAsReadDto dto);
        Task<ResponseDto> MarkAllAsReadAsync(int userId);

        // Individual notification methods
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto);
        Task<NotificationDto?> GetNotificationByIdAsync(int notificationId);
        Task<ResponseDto> DeleteNotificationAsync(int notificationId);
    }
}