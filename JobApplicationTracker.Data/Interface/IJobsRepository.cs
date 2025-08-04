using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobsRepository
{
    Task<IEnumerable<JobsDataModel>> GetAllJobsAsync();
    Task<JobsDataModel?> GetJobsByIdAsync(int jobId);
    Task<IEnumerable<JobsDataModel>> GetJobsByCompanyIdAsync(int companyId); // ✅ New method
    Task<ResponseDto> SubmitJobAsync(JobsDataModel jobsDto);
    Task<ResponseDto> DeleteJobAsync(int jobsId);
}