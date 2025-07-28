using System.ComponentModel.DataAnnotations.Schema;

namespace JobApplicationTracker.Data.DataModels
{
    [Table("Job")] // ✅ Add this line
    public class JobsDataModel
    {
        public int JobId { get; set; }
        public int CompanyId { get; set; } // Foreign Key
        public int? PostedByUserId { get; set; } // Foreign Key (nullable)
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public decimal? SalaryRangeMin { get; set; }
        public decimal? SalaryRangeMax { get; set; }
        public int JobTypeId { get; set; }
        public int ExperienceLevel { get; set; }
        public string? Responsibilities { get; set; }
        public string? Requirements { get; set; }
        public string? Benefits { get; set; }
        public DateTime? PostedAt { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public bool Status { get; set; }
        public int Views { get; set; }
    }
}
