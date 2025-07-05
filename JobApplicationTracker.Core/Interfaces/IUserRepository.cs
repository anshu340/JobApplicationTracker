using JobApplicationTracker.Core.Models;

namespace JobApplicationTracker.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);  // Returns the User domain entity
}