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

        public string? PhoneNumber { get; set; }
        public string? ResumeUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? LinkedinUrl { get; set; }
        public string? Location { get; set; }
        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public List<string>? PreferredJobTypes { get; set; }
        public List<string>? PreferredExperienceLevels { get; set; }
    }
