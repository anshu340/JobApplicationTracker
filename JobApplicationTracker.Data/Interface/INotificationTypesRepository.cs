
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface INotificationsTypesRepository
{
    Task<IEnumerable<NotificationTypesDataModel>> GetAllNotificationTypesAsync();
    Task<NotificationTypesDataModel> GetNotificationTypesByIdAsync(int notificationTypesId);
    Task<ResponseDto> SubmitNotificationTypesAsync(NotificationTypesDataModel notificationTypesDto);
    Task<ResponseDto> DeleteNotificationTypesAsync(int notificationTypesId);
}