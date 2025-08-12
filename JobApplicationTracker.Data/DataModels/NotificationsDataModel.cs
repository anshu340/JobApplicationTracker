namespace JobApplicationTracker.Data.DataModels;

public class NotificationsDataModel
{
    public Guid NotificationId { get; set; }
    public int UserId { get; set; } // Foreign Key
    public int NotificationTypeId { get; set; } // Foreign Key
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool? IsRead { get; set; } // Nullable in DB, though defaults exist
    public DateTime? CreatedAt { get; set; } // Nullable in DB, though defaults exist
    public string? LinkUrl { get; set; }
}