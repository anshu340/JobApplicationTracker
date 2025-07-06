namespace JobApplicationTracker.Data.DataModels;

public class ApplicationStatusesDataModel
{
    public int ApplicationStatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string? Description { get; set; }
}