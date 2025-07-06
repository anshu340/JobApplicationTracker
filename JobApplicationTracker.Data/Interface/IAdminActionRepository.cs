
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IAdminActionRepository
{
    Task<IEnumerable<AdminLogsDataModel>> GetAllAdminActionAsync();
    Task<AdminLogsDataModel> GetAdminActionByIdAsync(int adminActionId);
    Task<ResponseDto> SubmitAdminActionAsync(AdminLogsDataModel adminActionDto);
    Task<ResponseDto> DeleteAdminActionAsync(int actionId);
}


