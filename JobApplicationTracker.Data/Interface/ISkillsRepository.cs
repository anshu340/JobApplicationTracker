
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface ISkillsRepository
{
    Task<IEnumerable<SkillsDataModel>> GetAllSkillsAsync();

    Task<SkillsDataModel> GetSkillsByIdAsync(int skillsId);

    Task<SkillsDataModel> GetSkillsByIdAsync(int skillId);

    Task<IEnumerable<SkillsDataModel>> GetSkillsByUserIdAsync(int userId);
    Task<IEnumerable<int>> GetSkillsIdByUserIdAsync(int userId);
    Task<ResponseDto> SubmitSkillsAsync(SkillsDataModel skillsDto);
    Task<ResponseDto> DeleteSkillsAsync(int skillsId);
}