using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobApplicationRepository
{
    Task<IEnumerable<ApplicationsDataModel>> GetAllJobApplicationAsync();
    Task<ApplicationsDataModel> GetJobApplicationByIdAsync(int jobApplicationId);
    Task<IEnumerable<ApplicationsDataModel>> GetApplicationsByCompanyIdAsync(int companyId);
    Task<IEnumerable<ApplicationsDataModel>> GetJobApplicationsByUserIdAsync(int userId);
    Task<ResponseDto> SubmitJobApplicationAsync(ApplicationsDataModel jobApplicationDto);
    Task<ResponseDto> DeleteJobApplicationAsync(int jobApplicationId);
}