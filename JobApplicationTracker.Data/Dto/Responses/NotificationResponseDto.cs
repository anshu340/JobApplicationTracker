using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Responses
{
    public class NotificationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int NotificationsSent { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
