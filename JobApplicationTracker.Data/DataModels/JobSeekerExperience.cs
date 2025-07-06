namespace JobApplicationTracker.Data.DataModels;

public class JobSeekerExperience
{
    public int ExperienceId { get; set; }
    public int JobSeekerId { get; set; } // Foreign key 
    public string CompanyName { get; set; } = null!;
    public string Position { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; } // Nullable to allow for ongoing experience
    public string? Description { get; set; } // Optional description of the experience
}