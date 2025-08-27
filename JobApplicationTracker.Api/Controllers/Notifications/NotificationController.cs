using JobApplicationTracker.Business.Interface;
using JobApplicationTracker.Data.Dto.Requests;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;
using Microsoft.AspNetCore.Mvc;

namespace JobApplicationTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(
            INotificationService notificationService,
            ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        // POST: api/notification/send-job-notifications
        [HttpPost("send-job-notifications")]
        public async Task<ActionResult<NotificationResponseDto>> SendJobSkillNotifications(
            [FromBody] JobSkillNotificationDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.JobId <= 0)
                {
                    return BadRequest(new NotificationResponseDto
                    {
                        Success = false,
                        Message = "Invalid JobId provided"
                    });
                }

                var result = await _notificationService.SendJobSkillNotificationsAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendJobSkillNotifications endpoint");
                return StatusCode(500, new NotificationResponseDto
                {
                    Success = false,
                    Message = "An error occurred while sending notifications",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        // GET: api/notification/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid UserId");
                }

                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting notifications for user {userId}");
                return StatusCode(500, "An error occurred while retrieving notifications");
            }
        }

        // GET: api/notification/user/{userId}/unread
        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotifications(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid UserId");
                }

                var notifications = await _notificationService.GetUnreadNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread notifications for user {userId}");
                return StatusCode(500, "An error occurred while retrieving unread notifications");
            }
        }

        // GET: api/notification/user/{userId}/unread-count
        [HttpGet("user/{userId}/unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest("Invalid UserId");
                }

                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread count for user {userId}");
                return StatusCode(500, "An error occurred while getting unread count");
            }
        }

        // PUT: api/notification/mark-read
        [HttpPut("mark-read")]
        public async Task<ActionResult<ResponseDto>> MarkAsRead([FromBody] MarkAsReadDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _notificationService.MarkAsReadAsync(request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notification {request.NotificationId} as read");
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while marking notification as read"
                });
            }
        }

        // PUT: api/notification/user/{userId}/mark-all-read
        [HttpPut("user/{userId}/mark-all-read")]
        public async Task<ActionResult<ResponseDto>> MarkAllAsRead(int userId)
        {
            try
            {
                if (userId <= 0)
                {
                    return BadRequest(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid UserId"
                    });
                }

                var result = await _notificationService.MarkAllAsReadAsync(userId);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking all notifications as read for user {userId}");
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while marking all notifications as read"
                });
            }
        }

        // GET: api/notification/{notificationId}
        [HttpGet("{notificationId}")]
        public async Task<ActionResult<NotificationDto>> GetNotificationById(int notificationId)
        {
            try
            {
                if (notificationId <= 0)
                {
                    return BadRequest("Invalid NotificationId");
                }

                var notification = await _notificationService.GetNotificationByIdAsync(notificationId);

                if (notification == null)
                {
                    return NotFound("Notification not found");
                }

                return Ok(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting notification {notificationId}");
                return StatusCode(500, "An error occurred while retrieving the notification");
            }
        }

        // DELETE: api/notification/{notificationId}
        [HttpDelete("{notificationId}")]
        public async Task<ActionResult<ResponseDto>> DeleteNotification(int notificationId)
        {
            try
            {
                if (notificationId <= 0)
                {
                    return BadRequest(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Invalid NotificationId"
                    });
                }

                var result = await _notificationService.DeleteNotificationAsync(notificationId);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting notification {notificationId}");
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while deleting the notification"
                });
            }
        }

        // POST: api/notification/create
        [HttpPost("create")]
        public async Task<ActionResult<NotificationDto>> CreateNotification([FromBody] CreateNotificationDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _notificationService.CreateNotificationAsync(request);
                return CreatedAtAction(nameof(GetNotificationById), new { notificationId = result.NotificationId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification");
                return StatusCode(500, "An error occurred while creating the notification");
            }
        }
    }
}
