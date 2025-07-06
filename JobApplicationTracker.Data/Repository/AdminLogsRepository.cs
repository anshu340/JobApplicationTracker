using Dapper;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;

namespace JobApplicationTracker.Data.Repository;

public class AdminLogsRepository : IAdminLogsRepository
{
    private readonly IDatabaseConnectionService _connectionService;

    public AdminLogsRepository(IDatabaseConnectionService connectionService)
    {
        _connectionService = connectionService;
    }
    public async Task<IEnumerable<AdminLogsDataModel>> GetAllAdminLogsAsync()
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        var sql = """
              SELECT LogId, 
                     AdminId,
                     ActionId, 
                     Description,
                     ActionDate
              FROM AdminLogs
              """;

        return await connection.QueryAsync<AdminLogsDataModel>(sql).ConfigureAwait(false);
    }


    public async Task<AdminLogsDataModel> GetAdminLogsByIdAsync(int adminLogId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to fetch an admin log by ID
        var sql = """
              SELECT LogId, 
                     AdminId,
                     ActionId, 
                     Description,
                     ActionDate
              FROM AdminLogs
              WHERE LogId = @LogId
              """;

        var parameters = new DynamicParameters();
        parameters.Add("@LogId", adminLogId, DbType.Int32);

        return await connection.QueryFirstOrDefaultAsync<AdminLogsDataModel>(sql, parameters).ConfigureAwait(false);

    }
    public async Task<ResponseDto> SubmitAdminLogsAsync(AdminLogsDataModel adminLogsDto)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        string sql;

        if (adminLogsDto.LogId <= 0)
        {
            // Insert new log (assumes LogId is auto-incremented)
            sql = """
        INSERT INTO AdminLogs (AdminId, ActionId, Description, ActionDate)
        VALUES (@AdminId, @ActionId, @Description, @ActionDate);
        SELECT CAST(SCOPE_IDENTITY() AS INT);
        """;
        }
        else
        {
            // Update existing log
            sql = """
        UPDATE AdminLogs
        SET 
            AdminId = @AdminId,
            ActionId = @ActionId,
            Description = @Description,
            ActionDate = @ActionDate
        WHERE LogId = @LogId
        """;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@LogId", adminLogsDto.LogId, DbType.Int32);
        parameters.Add("@AdminId", adminLogsDto.AdminId, DbType.Int32);
        // parameters.Add("@ActionId", adminLogsDto.ActionId, DbType.Int32);
        parameters.Add("@Description", adminLogsDto.Description, DbType.String);
        // parameters.Add("@ActionDate", adminLogsDto.ActionDate, DbType.DateTime);

        var affectedRows = 0;

        if (adminLogsDto.LogId <= 0)
        {
            // Insert operation
            var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
            affectedRows = newId > 0 ? 1 : 0;
            adminLogsDto.LogId = newId; // Set the ID for the newly inserted record
        }
        else
        {
            // Update operation
            affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
        }

        return new ResponseDto
        {
            IsSuccess = affectedRows > 0,
            Message = affectedRows > 0 ? "Admins log submitted successfully." : "Failed to submit admin log."
        };
    }


    public async Task<ResponseDto> DeleteAdminLogsAsync(int logId)
    {
        await using var connection = await _connectionService.GetDatabaseConnectionAsync();

        // SQL query to delete an admin log by ID
        var sql = """DELETE FROM AdminLogs WHERE LogId = @LogId""";

        var parameters = new DynamicParameters();
        parameters.Add("@LogId", logId, DbType.Int32);

        var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);

        if (affectedRows <= 0)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = "Failed to delete admin log."
            };
        }

        return new ResponseDto
        {
            IsSuccess = true,
            Message = "Admins log deleted successfully."
        };
    }
}
