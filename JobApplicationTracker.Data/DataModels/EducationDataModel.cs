namespace JobApplicationTracker.Data.DataModels;

public class JobSeekerEducation
{
    public int EducationId { get; set; }
    public int JobSeekerId { get; set; } // Foreign key to JobSeekers
    public string University { get; set; } = null!;
    public string College { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string? FieldOfStudy { get; set; }
    public DateTime StartDate { get; set; }
    public bool Status { get; set; } //  education status either true or false, ongoing, finished
    public DateTime? EndDate { get; set; } 
    public double? Gpa { get; set; } 
}