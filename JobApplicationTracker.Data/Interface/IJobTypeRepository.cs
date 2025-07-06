
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface IJobTypeRepository
{
    Task<IEnumerable<JobTypesDataModel>> GetAllJobTypesAsync();
    Task<JobTypesDataModel> GetJobTypeByIdAsync(int jobTypeId);
    Task<ResponseDto> SubmitJobTypeAsync(JobTypesDataModel jobTypeDto);
    Task<ResponseDto> DeleteJobTypeAsync(int jobTypeId);
}