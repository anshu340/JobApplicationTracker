namespace JobApplicationTracker.Data.DataModels;

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
    public int JobTypeId { get; set; } // Foreign Key
    public string ExperienceLevel { get; set; } = null!;
    public string? Responsibilities { get; set; }
    public string? Requirements { get; set; }
    public string? Benefits { get; set; }
    public DateTime? PostedAt { get; set; } // Nullable in DB, though defaults exist
    public DateTime? ApplicationDeadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? Views { get; set; } // Nullable in DB, though defaults exist
}