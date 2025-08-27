using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Requests
{
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public int NotificationTypeId { get; set; }
        public int JobId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? LinkUrl { get; set; }
    }
}
