
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobSeekersEducationRepository
{
    Task<IEnumerable<JobSeekerEducation>> GetAllJobSeekerEducationAsync();
    Task<JobSeekerEducation> GetJobSeekerEducationByIdAsync(int jobSeekerEducationId);
    Task<ResponseDto> SubmitJobSeekerEducationAsync(JobSeekerEducation jobSeekerEducationDto);
    Task<ResponseDto> DeleteJobSeekerEducationAsync(int jobSeekerEducationId);
}