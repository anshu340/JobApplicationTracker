using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;


namespace JobApplicationTracker.Data.Interface
{
    public interface IExperienceRepository
    {
        Task<IEnumerable<ExperienceDto>> GetAllExperiencesAsync();
        Task<ExperienceDto?> GetExperienceByIdAsync(int id);

        Task<IEnumerable<ExperienceDataModel>> GetExperiencesByUserIdAsync(int userId);

        Task<ResponseDto> SubmitExperienceAsync(ExperienceDataModel experience);
        Task<ResponseDto> UpdateExperienceAsync(int id, ExperienceDataModel experience);
        Task<ResponseDto> DeleteExperienceAsync(int id);
    }
}