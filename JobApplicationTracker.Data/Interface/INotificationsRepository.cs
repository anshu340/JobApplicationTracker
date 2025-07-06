
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface INotificationsRepository
{
    Task<IEnumerable<NotificationsDataModel>> GetAllNotificationsAsync();
    Task<NotificationsDataModel> GetNotificationsByIdAsync(int notificationsId);
    Task<ResponseDto> SubmitNotificationsAsync(NotificationsDataModel notificationsDto);
    Task<ResponseDto> DeleteNotificationsAsync(int notificationsId);
}