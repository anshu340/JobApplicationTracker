using JobApplicationTracker.Data.Dto.Responses;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface
{
    public interface IEducationRepository
    {
        Task<IEnumerable<EducationDto>> GetAllEducationAsync();
        Task<EducationDto?> GetEducationByIdAsync(int educationId);
        Task<ResponseDto> SubmitEducationAsync(EducationDto dto);
        Task<ResponseDto> DeleteEducationAsync(int educationId);
        Task<IEnumerable<EducationDto>> GetEducationByUserIdAsync(int userId);
        Task<IEnumerable<int>> GetEducationIdsByUserIdAsync(int userId);

    }
}
