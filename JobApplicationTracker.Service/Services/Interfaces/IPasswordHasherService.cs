namespace JobApplicationTracker.Service.Services.Interfaces;
public interface IPasswordHasherService
{
    string HashPassword(string password);
}
