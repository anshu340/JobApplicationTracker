
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracker.Service.Services.Interfaces;

public interface IAuthenticationService
{
    string GenerateJwtToken(UsersDto user);
}
