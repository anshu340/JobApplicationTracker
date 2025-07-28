using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Requests
{
    public class JobDto
    {
        public int JobId { get; set; } // Optional for insert, required for update
        public int PostedByUserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Location { get; set; }
        public int JobTypeId { get; set; }
        public decimal SalaryRangeMin { get; set; }
        public decimal SalaryRangeMax { get; set; }
        public int ExperienceLevel { get; set; }
        public bool Status { get; set; }
        public DateTime PostedAt { get; set; }
        public DateTime ApplicationDeadline { get; set; }
    }
}