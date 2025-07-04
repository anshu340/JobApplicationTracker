

namespace JobApplicationTracke.Data.Dto;

public class UsersDto
{
    public int UserId { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } = null!;
    public string UserType { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public bool IsActive { get; set; } = true;  
}

