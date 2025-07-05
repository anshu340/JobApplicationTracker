namespace JobApplicationTracker.Data.Models.UserModels;

public class Admin
{
    public int AdminId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string Role { get; set; } = null!;
}