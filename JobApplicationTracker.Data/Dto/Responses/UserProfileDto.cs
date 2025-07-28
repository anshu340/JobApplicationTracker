using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Responses
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public int UserType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CompanyProfileDto? CompanyProfile { get; set; }
        public JobSeekersProfileDto? JobSeekerProfile { get; set; }
    }

}
