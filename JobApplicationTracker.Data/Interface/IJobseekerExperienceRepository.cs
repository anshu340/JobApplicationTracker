
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobSeekerExperienceRepository
{
    Task<IEnumerable<JobSeekerExperience>> GetAllJobSeekerExperienceAsync();
    Task<JobSeekerExperience> GetJobSeekerExperienceByIdAsync(int jobSeekerExperienceId);
    Task<ResponseDto> SubmitJobSeekerExperienceAsync(JobSeekerExperience jobSeekerExperienceDto);
    Task<ResponseDto> DeleteJobSeekerExperienceAsync(int jobSeekerExperienceId);
}