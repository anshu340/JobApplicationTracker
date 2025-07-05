namespace JobApplicationTracker.Data.Models.UserModels;

public class Company
{
    public int CompanyId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string WebsiteUrl { get; set; } = null!;
    public string LogoUrl { get; set; } = null!;
    public string Industry { get; set; } = null!;
    public string HeadQuarters { get; set; } = null!;
    public string ContactEmail { get; set; } = null!;
    public DateTime FoundedDate { get; set; } 
    public bool Status { get; set; }
}