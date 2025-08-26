using JobApplicationTracker.Data.Enums;

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

    // Helper property for type safety using your enum
    public ApplicationStatus Status
    {
        get => (ApplicationStatus)ApplicationStatus;
        set => ApplicationStatus = (int)value;
    }

    // Helper method to get status name
    public string GetStatusName()
    {
        return ApplicationStatus switch
        {
            1 => "Applied",
            2 => "Phone Screen",
            3 => "Rejected"
        };
    }

    // Helper method to validate if the status is valid
    public bool IsValidStatus()
    {
        return ApplicationStatus >= 1 && ApplicationStatus <= 3;
    }
}