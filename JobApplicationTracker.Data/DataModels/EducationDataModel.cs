namespace JobApplicationTracker.Data.DataModels;

public class EducationDataModel
{
    public int EducationId { get; set; }
    public string School { get; set; } = null!;
    public string Degree { get; set; } = null!;
    public string? FieldOfStudy { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentlyStudying { get; set; }
    public string? Description { get; set; }
    public decimal? GPA { get; set; }
}