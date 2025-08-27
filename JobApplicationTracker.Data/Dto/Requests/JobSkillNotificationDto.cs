using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Requests
{
    public class JobSkillNotificationDto
    {
        public int JobId { get; set; } // Foreign key does all the work!
    }
}
