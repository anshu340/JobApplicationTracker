namespace  JobApplicationTracker.Data.Models;
public class JobSeekerDataModel
{
    public Guid JobSeekerId { get; set; } // Matches DB column
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? ResumeUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? LinkedinProfile { get; set; } // Matches DB column 'linkedinProfile'
    public string? Location { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PreferredJobTypes { get; set; } // Stored as JSON string
    public string? PreferredExperienceLevels { get; set; } // Stored as JSON string
}