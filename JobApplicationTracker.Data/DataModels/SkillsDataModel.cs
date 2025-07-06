namespace JobApplicationTracker.Data.DataModels;

public class SkillsDataModel
{
    public int SkillId { get; set; }
    public string SkillName { get; set; } = null!;
    public string Category { get; set; }  = null!;
}