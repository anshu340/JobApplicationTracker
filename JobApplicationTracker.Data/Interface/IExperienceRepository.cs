using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface
{
    public interface IExperienceRepository
    {
        // Get operations
        Task<IEnumerable<ExperienceDto>> GetAllExperiencesAsync();
        Task<ExperienceDto?> GetExperienceByIdAsync(int experienceId);
        Task<IEnumerable<ExperienceDataModel>> GetExperiencesByUserIdAsync(int userId);

        // Submit operations (handles both insert and update)
        Task<ResponseDto> SubmitExperienceAsync(ExperienceDataModel experience);

        // Delete operation
        Task<ResponseDto> DeleteExperienceAsync(int experienceId);
    }
}