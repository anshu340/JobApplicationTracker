
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface INotificationsRepository
{
    Task<IEnumerable<NotificationsDto>> GetAllNotificationsAsync();
    Task<NotificationsDto> GetNotificationsByIdAsync(int notificationsId);
    Task<ResponseDto> SubmitNotificationsAsync(NotificationsDto notificationsDto);
    Task<ResponseDto> DeleteNotificationsAsync(int notificationsId);
}