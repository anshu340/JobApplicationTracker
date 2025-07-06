
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobSeekersRepository
{
    Task<IEnumerable<JobSeekersDataModel>> GetAllJobSeekersAsync();
    Task<JobSeekersDataModel> GetJobSeekersByIdAsync(int jobSeekerId);
    Task<ResponseDto> SubmitJobSeekersAsync(JobSeekersDataModel jobSeekerDto);
    Task<ResponseDto> DeleteJobSeekersAsync(int jobSeekerId);
}