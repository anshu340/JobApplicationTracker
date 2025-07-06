
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IAdminLogsRepository
{
    Task<IEnumerable<AdminLogsDataModel>> GetAllAdminLogsAsync();
    Task<AdminLogsDataModel> GetAdminLogsByIdAsync(int adminLogId);
    Task<ResponseDto> SubmitAdminLogsAsync(AdminLogsDataModel adminLogsDto);

    Task<ResponseDto> DeleteAdminLogsAsync(int logId);
}