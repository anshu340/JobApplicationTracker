
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface IJobsRepository
{
    Task<IEnumerable<JobsDto>> GetAllJobsAsync();
    Task<JobsDto> GetJobsByIdAsync(int jobId);
    Task<ResponseDto> SubmitJobAsync(JobsDto jobsDto);
    Task<ResponseDto> DeleteJobAsync(int jobsId);
}