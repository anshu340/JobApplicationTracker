
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface ISkillsRepository
{
    Task<IEnumerable<SkillsDto>> GetAllSkillsAsync();
    Task<SkillsDto> GetSkillsByIdAsync(int skillsId);
    Task<ResponseDto> SubmitSkillsAsync(SkillsDto skillsDto);
    Task<ResponseDto> DeleteSkillsAsync(int skillsId);
}