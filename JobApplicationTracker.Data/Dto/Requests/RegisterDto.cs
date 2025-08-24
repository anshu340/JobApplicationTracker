using JobApplicationTracker.Data.DataModels;

namespace JobApplicationTracker.Service.DTO.Requests;

public class RegisterDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Location { get; set; } = null!;
    public int? CompanyId { get; set; }
    public int? UserType { get; set; }
    public CompaniesDataModel? CompanyDto { get; set; }
    public DateTime? CreateDateTime { get; set; }

    public RegisterDto()
    {
        CreateDateTime = DateTime.Now;
    }
}