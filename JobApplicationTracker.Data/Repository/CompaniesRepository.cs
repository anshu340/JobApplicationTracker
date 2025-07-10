using Dapper;
using System.Data;
using System.Net;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;

namespace JobApplicationTracker.Data.Repository;

public class CompaniesRepository : ICompaniesRepository
{
    private readonly IDatabaseConnectionService _connectionService;
    public CompaniesRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<CompaniesDataModel>> GetAllCompaniesAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT CompanyId,
                         CompanyName,
                         Description,
                         WebsiteUrl,
                         CompanyLogo,
                         Industry,
                         Headquarters,
                         Location,
                         ContactEmail,
                         FoundedDate,
                         Status,
                         CreateDateTime
                  FROM Companies
                  """;

        return await connection.QueryAsync<CompaniesDataModel>(sql).ConfigureAwait(false);
    }


    public async Task<CompaniesDataModel> GetCompaniesByIdAsync(int companiesId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
                  SELECT CompanyId,
                         CompanyName,
                         Description,
                         WebsiteUrl,
                         CompanyLogo,
                         Industry,
                         Headquarters,
                         Location,
                         ContactEmail,
                         FoundedDate,
                         Status,
                         CreateDateTime
                  FROM Companies
                  WHERE CompanyId = @companiesId
                  """;

        var parameters = new DynamicParameters();
        parameters.Add("@companiesId", companiesId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<CompaniesDataModel>(sql, parameters).ConfigureAwait(false);
    }

       public async Task<ResponseDto> SubmitCompaniesAsync(CompaniesDataModel companiesDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;
        bool isInsert = companiesDto.CompanyId <= 0;

        if (isInsert)
        {
            sql = """
                  INSERT INTO Companies (
                      CompanyName, Description, WebsiteUrl, CompanyLogo, Industry,
                      Headquarters, Location, ContactEmail, FoundedDate, Status, CreateDateTime
                  )
                  VALUES (
                      @CompanyName, @Description, @WebsiteUrl, @CompanyLogo, @Industry,
                      @Headquarters, @Location, @ContactEmail, @FoundedDate, @Status, GETUTCDATE()
                  );
                  SELECT CAST(SCOPE_IDENTITY() AS INT);
                  """;
        }
        else
        {
            sql = """
                  UPDATE Companies
                  SET 
                      CompanyName = @CompanyName,
                      Description = @Description,
                      WebsiteUrl = @WebsiteUrl,
                      CompanyLogo = @CompanyLogo,
                      Industry = @Industry,
                      Headquarters = @Headquarters,
                      Location = @Location,
                      ContactEmail = @ContactEmail,
                      FoundedDate = @FoundedDate,
                      Status = @Status
                  WHERE CompanyId = @CompanyId
                  """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companiesDto.CompanyId);
        parameters.Add("@CompanyName", companiesDto.CompanyName);
        parameters.Add("@Description", companiesDto.Description);
        parameters.Add("@WebsiteUrl", companiesDto.WebsiteUrl);
        parameters.Add("@CompanyLogo", companiesDto.CompanyLogo);
        parameters.Add("@Industry", companiesDto.Industry);
        parameters.Add("@Headquarters", companiesDto.Headquarters);
        parameters.Add("@Location", companiesDto.Location);
        parameters.Add("@ContactEmail", companiesDto.ContactEmail);
        parameters.Add("@FoundedDate", companiesDto.FoundedDate);
        parameters.Add("@Status", companiesDto.Status);

        int affectedRows;
        if (isInsert)
        {
            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            companiesDto.CompanyId = newId;
        }
        else
        {
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0
                ? (isInsert ? "Company created successfully." : "Company updated successfully.")
                : "Failed to submit company."
        };
    }



    public async Task<int> CreateCompanyAsync(CompaniesDataModel request)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var query = @"INSERT INTO Companies (CompanyName, Description, Location) 
                    VALUES (@CompanyName, @Description, @Location);
                    SELECT SCOPE_IDENTITY();";
        
        var parameters = new DynamicParameters();
        parameters.Add("@CompanyName", request.CompanyName, DbType.String);
        parameters.Add("@Description", request.Description, DbType.String);
        parameters.Add("@Location", request.Location, DbType.String);
        
     int companyId =   await connection.ExecuteScalarAsync<int>(query, parameters).ConfigureAwait(false);
     return companyId;
    }
    
    
    public async Task<ResponseDto> DeleteCompanyAsync(int companiesId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // Check referential integrity first (example for JobApplications table)
        var refCheckSql = "SELECT COUNT(1) FROM JobApplications WHERE CompanyId = @CompanyId";
        var hasDependencies = await connection.ExecuteScalarAsync<bool>(refCheckSql, new { CompanyId = companiesId });

        if (hasDependencies)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Cannot delete company because it has associated job applications."
            };
        }

        // Delete the company
        var deleteSql = "DELETE FROM Companies WHERE CompanyId = @CompanyId";

        var parameters = new DynamicParameters();
        parameters.Add("@CompanyId", companiesId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(deleteSql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Companies not found or could not be deleted."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Companies deleted successfully.",

        };
    }

}