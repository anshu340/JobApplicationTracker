
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobApplicationRepository
{
    Task<IEnumerable<ApplicationsDataModel>> GetAllJobApplicationAsync();
    Task<ApplicationsDataModel> GetJobApplicationByIdAsync(int jobApplicationId);
    Task<ResponseDto> SubmitJobApplicationAsync(ApplicationsDataModel jobApplicationDto);
    Task<ResponseDto> DeleteJobApplicationAsync(int jobApplicationId);
}