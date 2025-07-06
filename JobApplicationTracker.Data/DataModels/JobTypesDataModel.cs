namespace JobApplicationTracker.Data.DataModels;

public class JobTypesDataModel
{
    public int JobTypeId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = string.Empty;
    
}