namespace JobApplicationTracker.Data.Dto.Responses;

public class UsersDtoResponse
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string UserType { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}