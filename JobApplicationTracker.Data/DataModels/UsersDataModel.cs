namespace JobApplicationTracker.Data.DataModels;
public class UsersDataModel
{
    public int UserId { get; set; }
    public int? CompanyId { get; set; }
    public string? Email { get; set; }  // Made nullable
    public string? PasswordHash { get; set; }  // Made nullable
    public int UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Experiences { get; set; }
    public string? FirstName { get; set; }  // Made nullable
    public string? LastName { get; set; }   // Made nullable
    public string? ProfilePicture { get; set; }
    public string? PhoneNumber { get; set; }
    public string? LinkedinProfile { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Skills { get; set; }
    public string? Education { get; set; }

    public UsersDataModel()
    {
        CreatedAt = DateTime.Now;
    }
}