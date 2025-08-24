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
    Task<ResponseDto> UploadCompanyLogoAsync(int companyId, string logoUrl);
    Task<bool> CompanyExistsAsync(int companyId);
    Task<string?> GetCompanyLogoAsync(int companyId);
}