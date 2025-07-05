using System.Text.Json;
using JobApplicationTracker.Core.Models;
using JobApplicationTracker.Data.Models;

namespace JobApplicationTracker.Data.Mappers;

public static class JobSeekerMapper
{
    public static JobSeeker ToDomainEntity(this JobSeekerDataModel dataModel)
    {
        if (dataModel is null) return null;

        var preferredJobTypes = string.IsNullOrEmpty(dataModel.PreferredJobTypes)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(dataModel.PreferredJobTypes);

        var preferredExperienceLevels = string.IsNullOrEmpty(dataModel.PreferredExperienceLevels)
            ? new List<string>()
            : JsonSerializer.Deserialize<List<string>>(dataModel.PreferredExperienceLevels);

        return JobSeeker.LoadFromPersistence(
            dataModel.JobSeekerId,
            dataModel.UserId,
            dataModel.FirstName,
            dataModel.LastName,
            dataModel.PhoneNumber,
            dataModel.ResumeUrl,
            dataModel.PortfolioUrl,
            dataModel.LinkedinProfile,
            dataModel.Location,
            dataModel.Headline,
            dataModel.Bio,
            dataModel.DateOfBirth,
            preferredJobTypes,
            preferredExperienceLevels
        );
    }

    public static JobSeekerDataModel ToDataModel(this JobSeeker domainEntity)
    {
        if (domainEntity == null) return null;

        var preferredJobTypesJson = JsonSerializer.Serialize(domainEntity.PreferredJobTypes);
        var preferredExperienceLevelsJson = JsonSerializer.Serialize(domainEntity.PreferredExperienceLevels);

        return new JobSeekerDataModel
        {
            JobSeekerId = domainEntity.JobSeekerId, // Map from Domain Entity to DataModel
            UserId = domainEntity.UserId,
            FirstName = domainEntity.FirstName,
            LastName = domainEntity.LastName,
            PhoneNumber = domainEntity.PhoneNumber,
            ResumeUrl = domainEntity.ResumeUrl,
            PortfolioUrl = domainEntity.PortfolioUrl,
            LinkedinProfile = domainEntity.LinkedinUrl, // Map from Domain Entity to DataModel
            Location = domainEntity.Location,
            Headline = domainEntity.Headline,
            Bio = domainEntity.Bio,
            DateOfBirth = domainEntity.DateOfBirth,
            PreferredJobTypes = preferredJobTypesJson,
            PreferredExperienceLevels = preferredExperienceLevelsJson
        };
    }
}
