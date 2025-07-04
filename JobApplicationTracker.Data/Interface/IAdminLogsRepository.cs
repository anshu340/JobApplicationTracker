
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface IAdminLogsRepository
{
    Task<IEnumerable<AdminLogsDto>> GetAllAdminLogsAsync();
    Task<AdminLogsDto> GetAdminLogsByIdAsync(int adminLogId);
    Task<ResponseDto> SubmitAdminLogsAsync(AdminLogsDto adminLogsDto);

    Task<ResponseDto> DeleteAdminLogsAsync(int logId);
}