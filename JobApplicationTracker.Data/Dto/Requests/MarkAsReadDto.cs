using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Requests
{
    public class MarkAsReadDto
    {
        public int NotificationId { get; set; }
        public bool IsRead { get; set; } = true;
    }
}
