namespace JobApplicationTracker.Core.Models;

public class JobSeeker
{
    public Guid JobSeekerId { get; private set; } 
    public Guid UserId { get; private set; } // Link to the User entity
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? ResumeUrl { get; private set; }
    public string? PortfolioUrl { get; private set; }
    public string? LinkedinUrl { get; private set; }
    public string? Location { get; private set; }
    public string? Headline { get; private set; }
    public string? Bio { get; private set; }
    public DateTime? DateOfBirth { get; private set; }

    public IReadOnlyList<string> PreferredJobTypes { get; private set; }
    public IReadOnlyList<string> PreferredExperienceLevels { get; private set; }

    // Private constructor for ORM rehydration or internal factory methods
    private JobSeeker()
    {
        PreferredJobTypes = new List<string>();
        PreferredExperienceLevels = new List<string>();
    }

    // Public constructor to create a NEW JobSeeker instance with required data
    // This constructor ensures that a JobSeeker is always created in a valid state.
    public JobSeeker(
        Guid userId, // UserId is required
        string firstName,
        string lastName,
        string? phoneNumber = null,
        string? resumeUrl = null,
        string? portfolioUrl = null,
        string? linkedinUrl = null,
        string? location = null,
        string? headline = null,
        string? bio = null,
        DateTime? dateOfBirth = null,
        IEnumerable<string>? preferredJobTypes = null,
        IEnumerable<string>? preferredExperienceLevels = null)
    {
        // Enforce invariants (business rules) here
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty.", nameof(lastName));

        JobSeekerId = Guid.NewGuid(); // Generate a new ID for a new entity
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        ResumeUrl = resumeUrl;
        PortfolioUrl = portfolioUrl;
        LinkedinUrl = linkedinUrl;
        Location = location;
        Headline = headline;
        Bio = bio;
        DateOfBirth = dateOfBirth;
        PreferredJobTypes = preferredJobTypes?.ToList() ?? new List<string>();
        PreferredExperienceLevels = preferredExperienceLevels?.ToList() ?? new List<string>();
    }

    // Example of a domain method
    public void UpdateProfile(string? phoneNumber, string? location, string? headline, string? bio)
    {
        // Add business validation here if needed
        PhoneNumber = phoneNumber;
        Location = location;
        Headline = headline;
        Bio = bio;
    }
    
    // Rehydration (Loading from Persistence): Happens when you fetch an existing entity from the database. 
    //     The entity is being brought back into memory with its existing state, including its ID. 
    //     This is the job of LoadFromPersistence.
    public static JobSeeker LoadFromPersistence(
        Guid id, Guid userId, string firstName, string lastName, string? phoneNumber,
        string? resumeUrl, string? portfolioUrl, string? linkedinUrl, string? location,
        string? headline, string? bio, DateTime? dateOfBirth,
        IEnumerable<string> preferredJobTypes, IEnumerable<string> preferredExperienceLevels)
    {
        var jobSeeker = new JobSeeker();
        // Directly assign properties, including the existing ID
        jobSeeker.JobSeekerId = id; 
        jobSeeker.UserId = userId;
        jobSeeker.FirstName = firstName;
        jobSeeker.LastName = lastName;
        jobSeeker.PhoneNumber = phoneNumber;
        jobSeeker.ResumeUrl = resumeUrl;
        jobSeeker.PortfolioUrl = portfolioUrl;
        jobSeeker.LinkedinUrl = linkedinUrl;
        jobSeeker.Location = location;
        jobSeeker.Headline = headline;
        jobSeeker.Bio = bio;
        jobSeeker.DateOfBirth = dateOfBirth;
        
        // Add to collections initialized in the private constructor
        ((List<string>)jobSeeker.PreferredJobTypes).AddRange(preferredJobTypes);
        ((List<string>)jobSeeker.PreferredExperienceLevels).AddRange(preferredExperienceLevels);
       
        return jobSeeker;
    }
}