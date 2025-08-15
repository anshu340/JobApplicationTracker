using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface
{
    public interface IExperienceRepository
    {
        
        Task<IEnumerable<ExperienceDataModel>> GetAllExperiencesAsync();

       
        Task<ExperienceDataModel?> GetExperienceByIdAsync(int id);

        
        Task<ResponseDto> SubmitExperienceAsync(ExperienceDataModel experience);

       
        Task<ResponseDto> UpdateExperienceAsync(int id, ExperienceDataModel experience);

     
        Task<ResponseDto> DeleteExperienceAsync(int id);
    }
}
