
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface IJobSeekersEducationRepository
{
    Task<IEnumerable<JobSeekerEducationDto>> GetAllJobSeekerEducationAsync();
    Task<JobSeekerEducationDto> GetJobSeekerEducationByIdAsync(int jobSeekerEducationId);
    Task<ResponseDto> SubmitJobSeekerEducationAsync(JobSeekerEducationDto jobSeekerEducationDto);
    Task<ResponseDto> DeleteJobSeekerEducationAsync(int jobSeekerEducationId);
}