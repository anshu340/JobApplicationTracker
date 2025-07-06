namespace JobApplicationTracker.Data.Dto.AuthDto;

public class SignUpDto
{
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
}