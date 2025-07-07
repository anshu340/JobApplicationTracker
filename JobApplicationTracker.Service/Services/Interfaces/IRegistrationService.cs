using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Service.DTO.Requests;

namespace JobApplicationTracker.Service.Services.Interfaces;

public interface IRegistrationService
{
    Task<ResponseDto> RegisterJobSeekerAsync(RegisterJobSeekerRequestDto request);
    Task<ResponseDto> RegisterCompanyAsync(RegisterCompanyRequestDto request);
    // Task<ResponseDto> RegisterAdminAsync(RegisterAdminRequestDto request);
}