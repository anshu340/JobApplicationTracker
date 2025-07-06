
using JobApplicationTracker.Data.DataModels;

namespace JobApplicationTracker.Service.Services.Interfaces;

public interface IAuthenticationService
{
    string GenerateJwtToken(UsersDataModel user);
}
