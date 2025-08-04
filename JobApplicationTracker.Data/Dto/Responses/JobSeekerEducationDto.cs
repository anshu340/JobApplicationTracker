using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplicationTracker.Dto
{
    public class JobSeekerEducationDto
    {
        public int EducationId { get; set; }
        public int JobSeekerId { get; set; }
        public string University { get; set; } = string.Empty;
        public string College { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public bool Status { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Gpa { get; set; }
    }
}


