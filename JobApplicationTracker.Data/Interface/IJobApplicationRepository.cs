
using JobApplicationTracke.Data.Dto;
namespace JobApplicationTracke.Data.Interface;

public interface IJobApplicationRepository
{
    Task<IEnumerable<JobApplicationDto>> GetAllJobApplicationAsync();
    Task<JobApplicationDto> GetJobApplicationByIdAsync(int jobApplicationId);
    Task<ResponseDto> SubmitJobApplicationAsync(JobApplicationDto jobApplicationDto);
    Task<ResponseDto> DeleteJobApplicationAsync(int jobApplicationId);
}