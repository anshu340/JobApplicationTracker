using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface ISkillsRepository
{
    Task<IEnumerable<SkillsDataModel>> GetAllSkillsAsync();
    Task<SkillsDataModel> GetSkillsByIdAsync(int skillId);
    Task<ResponseDto> SubmitSkillsAsync(SkillsDataModel skillsDto);
    Task<ResponseDto> DeleteSkillsAsync(int skillId);
}