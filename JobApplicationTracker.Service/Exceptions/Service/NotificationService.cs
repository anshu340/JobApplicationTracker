using JobApplicationTracker.Business.Interface;
using JobApplicationTracker.Data.Dto.Requests;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;
using Microsoft.Extensions.Logging;


namespace JobApplicationTracker.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService; 
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IEmailService emailService,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<NotificationResponseDto> SendJobSkillNotificationsAsync(JobSkillNotificationDto dto)
        {
            var response = new NotificationResponseDto();
            var errors = new List<string>();
            int notificationsSent = 0;

            try
            {
                // 1. Get job details using foreign key
                var job = await _notificationRepository.GetJobByIdAsync(dto.JobId);
                if (job == null)
                {
                    response.Success = false;
                    response.Message = "Job not found";
                    return response;
                }

                if (string.IsNullOrEmpty(job.Skills))
                {
                    response.Success = false;
                    response.Message = "Job has no skills defined";
                    return response;
                }

                // 2. Find users with matching skills
                var matchingUsers = await _notificationRepository.GetUsersBySkillsAsync(job.Skills);
                var usersList = matchingUsers.ToList();

                if (!usersList.Any())
                {
                    response.Success = true;
                    response.Message = "No users found with matching skills";
                    response.NotificationsSent = 0;
                    return response;
                }

                _logger.LogInformation($"Found {usersList.Count} users with matching skills for job {dto.JobId}");

                // 3. Create notifications and send emails for each user
                foreach (var user in usersList)
                {
                    try
                    {
                        // Create notification in database
                        var createNotificationDto = new CreateNotificationDto
                        {
                            UserId = user.UserId,
                            NotificationTypeId = 1, // Email type
                            JobId = dto.JobId,
                            Title = $"New Job Opportunity: {job.JobType}",
                            Message = CreateNotificationMessage(user, job),
                            LinkUrl = $"/jobs/{dto.JobId}/details"
                        };

                        var createdNotification = await _notificationRepository.CreateNotificationAsync(createNotificationDto);

                        // Send email notification
                        var emailSent = await _emailService.SendJobNotificationEmailAsync(user, job);

                        if (emailSent)
                        {
                            notificationsSent++;
                            _logger.LogInformation($"Notification sent to user {user.UserId} ({user.Email}) for job {dto.JobId}");
                        }
                        else
                        {
                            errors.Add($"Failed to send email to {user.Email}");
                            _logger.LogWarning($"Failed to send email to user {user.UserId} ({user.Email})");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error processing notification for user {user.Email}: {ex.Message}");
                        _logger.LogError(ex, $"Error sending notification to user {user.UserId}");
                    }
                }

                response.Success = notificationsSent > 0;
                response.Message = $"Successfully sent {notificationsSent} notifications out of {usersList.Count} matching users";
                response.NotificationsSent = notificationsSent;
                response.Errors = errors;

                _logger.LogInformation($"Job notification process completed. Sent {notificationsSent}/{usersList.Count} notifications");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendJobSkillNotificationsAsync");
                response.Success = false;
                response.Message = "An error occurred while sending notifications";
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
        {
            try
            {
                return await _notificationRepository.GetUserNotificationsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting notifications for user {userId}");
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            try
            {
                return await _notificationRepository.GetUnreadNotificationsAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread notifications for user {userId}");
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                return await _notificationRepository.GetUnreadCountAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread count for user {userId}");
                throw;
            }
        }

        public async Task<ResponseDto> MarkAsReadAsync(MarkAsReadDto dto)
        {
            try
            {
                return await _notificationRepository.MarkAsReadAsync(dto.NotificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notification {dto.NotificationId} as read");
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error marking notification as read: {ex.Message}",
                    Id = dto.NotificationId
                };
            }
        }

        public async Task<ResponseDto> MarkAllAsReadAsync(int userId)
        {
            try
            {
                return await _notificationRepository.MarkAllAsReadAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking all notifications as read for user {userId}");
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error marking all notifications as read: {ex.Message}",
                    Id = userId
                };
            }
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            try
            {
                return await _notificationRepository.CreateNotificationAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                throw;
            }
        }

        public async Task<NotificationDto?> GetNotificationByIdAsync(int notificationId)
        {
            try
            {
                return await _notificationRepository.GetNotificationByIdAsync(notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting notification {notificationId}");
                throw;
            }
        }

        public async Task<ResponseDto> DeleteNotificationAsync(int notificationId)
        {
            try
            {
                return await _notificationRepository.DeleteNotificationAsync(notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting notification {notificationId}");
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error deleting notification: {ex.Message}",
                    Id = notificationId
                };
            }
        }

        // Helper method to create personalized notification message
        private string CreateNotificationMessage(UsersDtoResponse user, JobDto job)
        {
            var userSkills = user.Skills?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(s => s.Trim())
                                        .ToList() ?? new List<string>();

            var jobSkills = job.Skills.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(s => s.Trim())
                                     .ToList();

            var matchingSkills = userSkills.Intersect(jobSkills, StringComparer.OrdinalIgnoreCase).ToList();

            var message = $"Hi {user.FirstName},\n\n" +
                         $"We found a new job opportunity that matches your profile!\n\n" +
                         $"Position: {job.JobType}\n" +
                         $"Location: {job.Location}\n" +
                         $"Employment Type: {job.EmpolymentType}\n";

            if (matchingSkills.Any())
            {
                message += $"Matching Skills: {string.Join(", ", matchingSkills)}\n";
            }

            if (job.SalaryRangeMin > 0 && job.SalaryRangeMax > 0)
            {
                message += $"Salary Range: ${job.SalaryRangeMin:N0} - ${job.SalaryRangeMax:N0}\n";
            }

            message += "\nDon't miss this opportunity! Click to view details and apply.";

            return message;
        }
    }
}