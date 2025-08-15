using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;

namespace JobApplicationTracker.Data.Interface;

public interface ICompaniesRepository
{
    Task<IEnumerable<CompaniesDataModel>> GetAllCompaniesAsync();
    Task<CompaniesDataModel> GetCompaniesByIdAsync(int companiesId);
    Task<ResponseDto> SubmitCompaniesAsync(CompaniesDataModel companiesDto);
    Task<int> CreateCompanyAsync(CompaniesDataModel request);
    Task<ResponseDto> DeleteCompanyAsync(int companiesId);
    Task<ResponseDto> UpdateCompanyLogoAsync(int companyId, string logoUrl);
}