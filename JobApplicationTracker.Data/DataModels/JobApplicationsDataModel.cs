//using JobApplicationTracker.Data.Enums;

namespace JobApplicationTracker.Data.DataModels;

public class ApplicationsDataModel
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; }
    public int JobId { get; set; }
    public int ApplicationStatus { get; set; }
    public DateTime ApplicationDate { get; set; }
    public string? CoverLetter { get; set; }
    public string? ResumeFile { get; set; }
    public decimal? SalaryExpectation { get; set; }
    public DateTime? AvailableStartDate { get; set; }
    public DateTime CreatedAt { get; set; }
}