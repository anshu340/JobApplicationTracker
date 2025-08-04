namespace JobApplicationTracker.Data.DataModels;

public class JobSeekerSkill
{
    public int JobSeekerSkillId { get; set; }
    public int JobSeekerId { get; set; }
    public int SkillId { get; set; }
    public int ProficiencyLevel { get; set; }
}
