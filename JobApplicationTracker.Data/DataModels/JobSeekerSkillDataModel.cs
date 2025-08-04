namespace JobApplicationTracker.Data.DataModels;

public class JobSeekerSkills
{
    public int JobSeekerSkillsId { get; set; }
    public int JobSeekerId { get; set; }
    public int SkillId { get; set; }
    public int ProficiencyLevel { get; set; }
}


