using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Service.DTO.Requests;

public class RegisterJobSeekerRequestDto
{
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [Required, StringLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, StringLength(100)]
        public string LastName { get; set; } = null!;

        public string? PhoneNumber { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        // public string? ResumeUrl { get; set; }  = string.Empty;
        // public string? PortfolioUrl { get; set; } = string.Empty;
        // public string? LinkedinUrl { get; set; } = string.Empty;
        // public string? Headline { get; set; } = string.Empty;
        // public string? Bio { get; set; } = string.Empty;
        // public DateTime? DateOfBirth { get; set; }
        // public List<string>? PreferredJobTypes { get; set; } = new List<string>();
        // public List<string>? PreferredExperienceLevels { get; set; } = new List<string>();
} 
