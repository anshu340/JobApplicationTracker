namespace JobApplicationTracker.Data.Dto.Responses;

public class UsersDtoResponse
{
    public int UserId { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserType { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? ProfilePicture { get; set; }
    public string? ResumeUrl { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? LinkedinProfile { get; set; }
    public string? Location { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }


    public string? Skills { get; set; }
    public string? Education { get; set; }


    public string? Skills { get; set; }
    public string? Education { get; set; }


}