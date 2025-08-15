using JobApplicationTracker.Dto; 
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobSeekersEducationRepository
{
    Task<IEnumerable<JobSeekerEducationDto>> GetAllJobSeekerEducationAsync(); 

    Task<JobSeekerEducationDto> GetJobSeekerEducationByIdAsync(int jobSeekerEducationId); 

    Task<ResponseDto> SubmitJobSeekerEducationAsync(JobSeekerEducationDto jobSeekerEducationDto);

    Task<ResponseDto> DeleteJobSeekerEducationAsync(int jobSeekerEducationId);
}
