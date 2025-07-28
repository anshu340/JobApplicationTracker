using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Service.DTO.Requests;

namespace JobApplicationTracker.Service.Services.Interfaces;

public interface IRegistrationService
{
    Task<ResponseDto> RegisterUserAsync(RegisterDto request);

}