using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Data.DataModels;

public class JobsDataModel
{
    public int JobId { get; set; }
    public int CompanyId { get; set; }
    public int PostedByUserId { get; set; }
    public string JobType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal SalaryRangeMin { get; set; }
    public decimal SalaryRangeMax { get; set; }
    public string EmpolymentType { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
    public string Responsibilities { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string Benefits { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
    public DateTime ApplicationDeadline { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Views { get; set; }
    public string Skills { get; set; } = string.Empty;

    // ✅ Add IsPublished property
    public bool IsPublished { get; set; } = false;
}