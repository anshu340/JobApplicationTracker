
using JobApplicationTracke.Data.Dto;
namespace JobApplicationTracke.Data.Interface;

public interface IJobTypeRepository
{
    Task<IEnumerable<JobTypeDto>> GetAllJobTypesAsync();
    Task<JobTypeDto> GetJobTypeByIdAsync(int jobTypeId);
    Task<ResponseDto> SubmitJobTypeAsync(JobTypeDto jobTypeDto);
    Task<ResponseDto> DeleteJobTypeAsync(int jobTypeId);
}