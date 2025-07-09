namespace JobApplicationTracker.Data.DataModels;

public class CompaniesDataModel
{
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? LogoUrl { get; set; }
    public string? Industry { get; set; }
    public string? Headquarters { get; set; }
    public string? Location { get; set; }
    public string? ContactEmail { get; set; }
    public DateTime? FoundedDate { get; set; }
    public string Status { get; set; } = string.Empty;
}