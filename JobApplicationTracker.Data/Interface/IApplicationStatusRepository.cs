using JobApplicationTracker.Data.Dto;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface
{
    public interface IApplicationStatusRepository
    {
        Task<IEnumerable<ApplicationStatusDto>> GetAllApplicationStatusesAsync();
        Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(int id);
        Task<ApplicationStatusDto> CreateApplicationStatusAsync(ApplicationStatusDto applicationStatus);
        Task<bool> DeleteApplicationStatusAsync(int id);
        Task<bool> ApplicationStatusExistsAsync(int id);
    }
}