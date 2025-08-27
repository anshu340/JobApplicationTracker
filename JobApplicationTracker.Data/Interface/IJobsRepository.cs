using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobsRepository
{
    Task<IEnumerable<JobsDataModel>> GetAllJobsAsync();
    Task<IEnumerable<JobsDataModel>> GetActiveJobsForUsersAsync();
    Task<JobsDataModel?> GetJobsByIdAsync(int jobId);
    Task<IEnumerable<JobsDataModel>> GetJobsByCompanyIdAsync(int companyId);
    Task<ResponseDto> SubmitJobAsync(JobsDataModel jobsDto);
    Task<ResponseDto> DeleteJobAsync(int jobsId);

    Task<ResponseDto> PublishJobAsync(int jobId);
    Task<ResponseDto> UnpublishJobAsync(int jobId);
}