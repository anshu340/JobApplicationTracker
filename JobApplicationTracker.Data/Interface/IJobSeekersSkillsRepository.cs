
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobSeekersSkillsRepository
{
    Task<IEnumerable<JobSeekerSkills>> GetAllJobSeekerSkillsAsync();
    Task<JobSeekerSkills> GetJobSeekerSkillsByIdAsync(int jobSeekerSkillsId);
    Task<ResponseDto> SubmitJobSeekerSkillsAsync(JobSeekerSkills jobSeekerSkillsDto);
    Task<ResponseDto> DeleteJobSeekerSkillsAsync(int jobSeekerSkillsId);
}