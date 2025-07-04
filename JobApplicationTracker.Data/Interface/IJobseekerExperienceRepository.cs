
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface IJobSeekerExperienceRepository
{
    Task<IEnumerable<JobSeekerExperienceDto>> GetAllJobSeekerExperienceAsync();
    Task<JobSeekerExperienceDto> GetJobSeekerExperienceByIdAsync(int jobSeekerExperienceId);
    Task<ResponseDto> SubmitJobSeekerExperienceAsync(JobSeekerExperienceDto jobSeekerExperienceDto);
    Task<ResponseDto> DeleteJobSeekerExperienceAsync(int jobSeekerExperienceId);
}