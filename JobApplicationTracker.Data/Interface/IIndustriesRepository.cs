
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface IIndustriesRepository
{
    Task<IEnumerable<IndustriesDto>> GetAllIndustriesAsync();
    Task<IndustriesDto> GetIndustryByIdAsync(int industryId);
    Task<ResponseDto> SubmitIndustriesAsync(IndustriesDto industriesDto);
    Task<ResponseDto> DeleteIndustriesAsync(int industryId);
}