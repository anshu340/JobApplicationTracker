using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Responses
{
    public class JobApplicationDto
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public int UserId { get; set; }
        public int ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string? CoverLetter { get; set; }
        public string? ResumeFile { get; set; }
        public decimal? SalaryExpectation { get; set; }
        public DateTime? AvailableStartDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
