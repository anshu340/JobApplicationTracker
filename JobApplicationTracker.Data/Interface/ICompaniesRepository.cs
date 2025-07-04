
using JobApplicationTracke.Data.Dto;

namespace JobApplicationTracke.Data.Interface;

public interface ICompaniesRepository
{
    Task<IEnumerable<CompaniesDto>> GetAllCompaniesAsync();
    Task<CompaniesDto> GetCompaniesByIdAsync(int companiesId);
    Task<ResponseDto> SubmitCompaniesAsync(CompaniesDto companiesDto);

    Task<ResponseDto> DeleteCompanyAsync(int companiesId);
    
}