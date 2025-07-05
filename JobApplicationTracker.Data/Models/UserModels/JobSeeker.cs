// namespace JobApplicationTracker.Data.Models.UserModels;
//
// public class JobSeeker 
// {
//     public Guid JobSeekerId { get; private set; } 
//     public Guid UserId { get; private set; } // Link to the User entity
//     public string FirstName { get; private set; }
//     public string LastName { get; private set; }
//     public string? PhoneNumber { get; private set; }
//     public string? ResumeUrl { get; private set; }
//     public string? PortfolioUrl { get; private set; }
//     public string? LinkedinUrl { get; private set; } 
//     public string? Location { get; private set; }
//     public string? Headline { get; private set; }
//     public string? Bio { get; private set; }
//     public DateTime? DateOfBirth { get; private set; }
//
//     // These would be proper C# collections/enums in the domain, not raw JSON strings
//     public IReadOnlyList<string> PreferredJobTypes { get; private set; }
//     public IReadOnlyList<string> PreferredExperienceLevels { get; private set; }
//
//     // Constructor for creating new JobSeekers (enforces invariants)
//     public JobSeeker(Guid jobSeekerId, Guid userId, string firstName, string lastName, string? phoneNumber, 
//                      string? resumeUrl, string? portfolioUrl, string? linkedinUrl, string? location, 
//                      string? headline, string? bio, DateTime? dateOfBirth, 
//                      IEnumerable<string> preferredJobTypes, IEnumerable<string> preferredExperienceLevels)
//     {
//         JobSeekerId = jobSeekerId;
//         UserId = userId;
//         FirstName = firstName;
//         LastName = lastName;
//         PhoneNumber = phoneNumber;
//         ResumeUrl = resumeUrl;
//         PortfolioUrl = portfolioUrl;
//         LinkedinUrl = linkedinUrl;
//         Location = location;
//         Headline = headline;
//         Bio = bio;
//         DateOfBirth = dateOfBirth;
//         PreferredJobTypes = preferredJobTypes?.ToList() ?? new List<string>();
//         PreferredExperienceLevels = preferredExperienceLevels?.ToList() ?? new List<string>();
//     }
//     
//     public void UpdateContactInfo(string newPhoneNumber, string newLocation)
//     {
//         // Add validation or business rules here
//         PhoneNumber = newPhoneNumber;
//         Location = newLocation;
//     }
//
//     // Private constructor for rehydration from persistence (e.g., used by mappers)
//     // Could also be achieved with private setters and an internal constructor.
//     private JobSeeker() 
//     {
//         PreferredJobTypes = new List<string>();
//         PreferredExperienceLevels = new List<string>();
//     }
// }