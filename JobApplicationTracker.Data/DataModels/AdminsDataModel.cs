namespace JobApplicationTracker.Data.DataModels;

public class AdminsDataModel
{
    public int AdminId { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Role { get; set; } = null!;
}