using Dapper;
using System.Data;
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
                         CompanyLogo,
                         IndustryId,
                         CompanySizeId,
                         Website,
                         Location,
                         Description,
                         CreatedAt,
                         UpdatedAt
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
                     CompanyLogo,
                     IndustryId,
                     CompanySizeId,
                     Website,
                     Location,
                     Description,
                     CreatedAt,
                     UpdatedAt
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
            // Insert new company
            sql = """
        INSERT INTO Companies (
            CompanyName, 
            CompanyLogo,
            IndustryId,
            Website,
            Location,
            Description
        )
        VALUES (
            @CompanyName,
            @CompanyLogo,
            @IndustryId,
            @Website,
            @Location,
            @Description
        );
        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;
        }
        else
        {
            // Update existing company
            sql = """
        UPDATE Companies
        SET 
            CompanyName = @CompanyName,
            CompanyLogo = @CompanyLogo,
            IndustryId = @IndustryId,
            CompanySizeId = @CompanySizeId,
            Website = @Website,
            Location = @Location,
            Description = @Description,
            UpdatedAt = GETUTCDATE()
        WHERE CompanyId = @CompanyId
        """;
        }

        var parameters = new DynamicParameters();
        //parameters.Add("@CompanyId", companiesDto.CompanyId, DbType.Int32);
        parameters.Add("@CompanyName", companiesDto.Name, DbType.String);
        parameters.Add("@CompanyLogo", companiesDto.LogoUrl, DbType.String);
        parameters.Add("@Industry", companiesDto.Industry, DbType.Int32);
        //parameters.Add("@CompanySizeId", companiesDto.CompanySizeId, DbType.Int32);
        parameters.Add("@WebsiteUrl", companiesDto.WebsiteUrl, DbType.String);
        parameters.Add("@HeadQuarters", companiesDto.Headquarters, DbType.String);
        parameters.Add("@Location", companiesDto.Location, DbType.String);
        parameters.Add("@Description", companiesDto.Description, DbType.String);

        var affectedRows = 0;

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
                ? (isInsert ? "Companies created successfully." : "Companies updated successfully.")
                : "Failed to submit company.",
            
        };
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