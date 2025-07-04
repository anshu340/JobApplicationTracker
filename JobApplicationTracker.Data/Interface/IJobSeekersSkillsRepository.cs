
using JobApplicationTracke.Data.Dto;
namespace JobApplicationTracke.Data.Interface;

public interface IJobSeekersSkillsRepository
{
    Task<IEnumerable<JobSeekerSkillsDto>> GetAllJobSeekerSkillsAsync();
    Task<JobSeekerSkillsDto> GetJobSeekerSkillsByIdAsync(int jobSeekerSkillsId);
    Task<ResponseDto> SubmitJobSeekerSkillsAsync(JobSeekerSkillsDto jobSeekerSkillsDto);
    Task<ResponseDto> DeleteJobSeekerSkillsAsync(int jobSeekerSkillsId);
}