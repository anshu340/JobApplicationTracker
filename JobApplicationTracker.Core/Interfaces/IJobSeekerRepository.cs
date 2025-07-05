using JobApplicationTracker.Core.Models;

namespace JobApplicationTracker.Core.Interfaces;

public interface IJobSeekerRepository
{
    Task AddAsync(JobSeeker jobSeeker);
    Task<JobSeeker?> GetByUserIdAsync(Guid userId);
}