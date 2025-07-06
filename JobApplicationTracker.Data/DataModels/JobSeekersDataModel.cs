namespace  JobApplicationTracker.Data.DataModels;
public class JobSeekersDataModel
{
    public int JobSeekerId { get; set; } 
    public int UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfilePicture { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ResumeUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? LinkedinProfile { get; set; } 
    public string? Location { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? PreferredJobTypes { get; set; } 
    public string? PreferredExperienceLevels { get; set; } 
}