namespace JobApplicationTracker.Data.DataModels;

public class NotificationTypesDataModel
{
    public int NotificationTypeId { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
}