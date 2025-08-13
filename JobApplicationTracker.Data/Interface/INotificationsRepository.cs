
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using System;

namespace JobApplicationTracker.Data.Interface
{
    public interface INotificationsRepository
    {
        Task<IEnumerable<NotificationsDataModel>> GetAllNotificationsAsync();
        Task<NotificationsDataModel> GetNotificationsByIdAsync(Guid notificationsId);
        Task<ResponseDto> SubmitNotificationsAsync(NotificationsDataModel notificationsDto);
        Task<ResponseDto> DeleteNotificationsAsync(Guid notificationsId);
    }
}
