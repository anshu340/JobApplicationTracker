namespace JobApplicationTracker.Data.DataModels;

public class UsersDataModel
{
    public int UserId { get; set; }
    public int? CompanyId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public int UserType { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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

    public UsersDataModel()
    {
        CreatedAt = DateTime.Now;
    }
}