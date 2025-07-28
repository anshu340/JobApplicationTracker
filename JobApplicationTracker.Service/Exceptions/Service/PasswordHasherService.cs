using BCrypt.Net;
using JobApplicationTracker.Service.Services.Interfaces;

namespace JobApplicationTracker.Service.Services.Service;
public class PasswordHasherService : IPasswordHasherService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
