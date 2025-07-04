
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface IJobSeekersRepository
{
    Task<IEnumerable<JobSeekersDto>> GetAllJobSeekersAsync();
    Task<JobSeekersDto> GetJobSeekersByIdAsync(int jobSeekerId);
    Task<ResponseDto> SubmitJobSeekersAsync(JobSeekersDto jobSeekerDto);
    Task<ResponseDto> DeleteJobSeekersAsync(int jobSeekerId);
}