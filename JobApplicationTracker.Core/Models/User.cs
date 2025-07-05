namespace JobApplicationTracker.Core.Models;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string UserType { get; private set; } = null!; 

    private User() {} // For ORM
    public User(Guid id, string email, string userType)
    {
        Id = id;
        Email = email;
        UserType = userType;
    }
}