using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Dto.Requests
{
    public class JobDto
    {
        public int JobId { get; set; } 
        public int PostedByUserId { get; set; }
        public string JobType { get; set; }
        public string Description { get; set; }
        public string Requirements { get; set; }
        public string Location { get; set; }
        public string EmpolymentType { get; set; }
        public decimal SalaryRangeMin { get; set; }
        public decimal SalaryRangeMax { get; set; }
        public string ExperienceLevel { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime PostedAt { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public string Skills { get; set; } 
    }
}