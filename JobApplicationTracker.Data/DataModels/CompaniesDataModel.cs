namespace JobApplicationTracker.Data.DataModels;

public class CompaniesDataModel
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? WebsiteUrl { get; set; }
    public string? CompanyLogo { get; set; }

    public string? Location { get; set; }
    public string? ContactEmail { get; set; }

    public string Status { get; set; } = string.Empty;

}