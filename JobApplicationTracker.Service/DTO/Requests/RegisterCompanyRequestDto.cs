namespace JobApplicationTracker.Service.DTO.Requests;

public class RegisterCompanyRequestDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string CompanyName { get; set; }  = null!;
    public string Location { get; set; } = null!;
    public string Description { get; set; } = null!;
}