using JobApplicationTracker.Data.Dto.Requests;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Business.Interface
{
    public interface IEmailService
    {
        Task<bool> SendJobNotificationEmailAsync(UsersDtoResponse user, JobDto job);
    }
}