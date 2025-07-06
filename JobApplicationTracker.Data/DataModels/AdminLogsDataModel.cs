namespace JobApplicationTracker.Data.DataModels;

public class AdminLogsDataModel
{
    public int LogId { get; set; }
    public int AdminId { get; set; } // Foreign Key
    public string ActionType { get; set; } = string.Empty; // edited, updated, deleted, created
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Description { get; set; }
    public string? IpAddress { get; set; }
    public DateTime? Timestamp { get; set; } 
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}