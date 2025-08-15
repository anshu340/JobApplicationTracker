using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Responses
{
    public class CompanyProfileDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? CompanyLogo { get; set; }
        public string? Location { get; set; }
        public string? ContactEmail { get; set; }
    }
}
