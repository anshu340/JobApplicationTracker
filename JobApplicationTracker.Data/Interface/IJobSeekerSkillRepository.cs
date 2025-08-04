
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobSeekerSkillRepository
{
    Task<IEnumerable<JobSeekerSkill>> GetAllJobSeekerSkillsAsync();
    Task<JobSeekerSkill> GetJobSeekerSkillsByIdAsync(int jobSeekerSkillId);
    Task<IEnumerable<JobSeekerSkill>> GetJobSeekerSkillsByJobSeekerIdAsync(int jobSeekerId);
    Task<ResponseDto> SubmitJobSeekerSkillsAsync(JobSeekerSkill jobSeekerSkill);
    Task<ResponseDto> DeleteJobSeekerSkillsAsync(int jobSeekerSkillId);
    Task<ResponseDto> DeleteJobSeekerSkillsByJobSeekerIdAsync(int jobSeekerId);
}