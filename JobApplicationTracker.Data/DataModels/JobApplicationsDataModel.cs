namespace JobApplicationTracker.Data.DataModels;

public class ApplicationsDataModel
{
    public int ApplicationId { get; set; }
    public int UserId { get; set; } // Changed from JobSeekerId to UserId
    public int JobId { get; set; } // Foreign Key
    public int ApplicationStatusId { get; set; } // Foreign Key
    public DateTime? AppliedAt { get; set; } // Nullable in DB, though defaults exist
    public string? CoverLetterText { get; set; }
    public string? CoverLetterUrl { get; set; }
    public string? Feedback { get; set; }
    public DateTime? LastUpdatedAt { get; set; } // Nullable in DB, though defaults exist
}