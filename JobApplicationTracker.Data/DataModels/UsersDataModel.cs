namespace JobApplicationTracker.Data.DataModels;

public class UsersDataModel
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string UserType { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UsersDataModel()
    {
        CreatedAt = DateTime.Now;
    }
}