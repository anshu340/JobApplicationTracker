using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobApplicationRepository
{
    Task<IEnumerable<JobApplicationsDataModel>> GetAllJobApplicationAsync();
    Task<JobApplicationsDataModel> GetJobApplicationByIdAsync(int jobApplicationId);
    Task<IEnumerable<JobApplicationsDataModel>> GetApplicationsByCompanyIdAsync(int companyId);
    Task<IEnumerable<JobApplicationsDataModel>> GetJobApplicationsByUserIdAsync(int userId);
    Task<ResponseDto> SubmitJobApplicationAsync(JobApplicationsDataModel jobApplicationDto);
    Task<ResponseDto> DeleteJobApplicationAsync(int jobApplicationId);
    Task<ResponseDto> AcceptJobApplicationAsync(int jobApplicationId);
    Task<ResponseDto> RejectJobApplicationAsync(int jobApplicationId, string? rejectionReason = null);
    Task<IEnumerable<JobApplicationsDataModel>> GetAcceptedJobApplicationsByUserIdAsync(int userId);
    Task<IEnumerable<JobApplicationsDataModel>> GetRejectedJobApplicationsByUserIdAsync(int userId);
}