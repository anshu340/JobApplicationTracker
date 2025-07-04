
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface INotificationsTypesRepository
{
    Task<IEnumerable<NotificationTypesDto>> GetAllNotificationTypesAsync();
    Task<NotificationTypesDto> GetNotificationTypesByIdAsync(int notificationTypesId);
    Task<ResponseDto> SubmitNotificationTypesAsync(NotificationTypesDto notificationTypesDto);
    Task<ResponseDto> DeleteNotificationTypesAsync(int notificationTypesId);
}