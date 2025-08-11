using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Responses
{
    public class UsersProfileDto
    {

        public int UserId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? ProfilePicture { get; set; }
        public string? ResumeUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? LinkedinProfile { get; set; }
        public string? Location { get; set; }
        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public int CompanyId { get; set; }
        public string Email { get; set; } = null!;
        public int UserType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Skills { get; set; }
        public string? Education { get; set; }
        public string? PreferredJobTypes { get; set; }
        public string? PreferredExperienceLevels { get; set; }

        public CompanyProfileDto? CompanyProfile { get; set; }

    }
}
