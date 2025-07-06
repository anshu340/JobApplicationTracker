//
// using Dapper;
// using JobApplicationTracke.Data.Dto;
// using JobApplicationTracke.Data.Interface;
// using System.Data;
//
// namespace JobApplicationTracker.Data.Repository;
//  
// public class AdminActionService : IAdminActionRepository
// {
//     private readonly IDatabaseConnectionService _connectionService;
//
//     public AdminActionService(IDatabaseConnectionService connectionService)
//     {
//         _connectionService = connectionService;
//     }
//     public async Task<IEnumerable<AdminActionsDto>> GetAllAdminActionAsync()
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         var sql = """
//               SELECT ActionId, 
//                      ActionName, 
//                      Description 
//               FROM AdminAction
//               """;
//
//         return await connection.QueryAsync<AdminActionsDto>(sql).ConfigureAwait(false);
//     }
//
//     public async Task<AdminActionsDto> GetAdminActionByIdAsync(int adminActionId)
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         // SQL query to fetch an admin action by ID
//         var sql = """
//               SELECT ActionId,
//                      ActionName,
//                      Description
//               FROM AdminAction
//               WHERE ActionId = @ActionId
//               """;
//
//         var parameters = new DynamicParameters();
//         parameters.Add("@ActionId", adminActionId, DbType.Int32); // Corrected parameter name
//
//         return await connection.QueryFirstOrDefaultAsync<AdminActionsDto>(sql, parameters).ConfigureAwait(false);
//     }
//     public async Task<ResponseDto> SubmitAdminActionAsync(AdminActionsDto adminActionDto)
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         string sql;
//
//         if (adminActionDto.ActionId <= 0)
//         {
//             // Insert new admin action (assumes ActionId is auto-incremented)
//             sql = """
//         INSERT INTO AdminAction (ActionName, Description)
//         VALUES (@ActionName, @Description);
//         SELECT CAST(SCOPE_IDENTITY() AS INT);
//         """;
//         }
//         else
//         {
//             // Update existing admin action
//             sql = """
//         UPDATE AdminAction
//         SET 
//             ActionName = @ActionName,
//             Description = @Description
//         WHERE ActionId = @ActionId
//         """;
//         }
//
//         var parameters = new DynamicParameters();
//         parameters.Add("@ActionId", adminActionDto.ActionId, DbType.Int32);
//         parameters.Add("@ActionName", adminActionDto.ActionName, DbType.String);
//         parameters.Add("@Description", adminActionDto.Description, DbType.String);
//
//         var affectedRows = 0;
//
//         if (adminActionDto.ActionId <= 0)
//         {
//             // Insert operation
//             var newId = await connection.QuerySingleAsync<int>(sql, parameters).ConfigureAwait(false);
//             affectedRows = newId > 0 ? 1 : 0;
//             adminActionDto.ActionId = newId; // Set the ID for the newly inserted record
//         }
//         else
//         {
//             // Update operation
//             affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
//         }
//
//         return new ResponseDto
//         {
//             IsSuccess = affectedRows > 0,
//             Message = affectedRows > 0 ? "Admins action submitted successfully." : "Failed to submit admin action."
//         };
//     }
//
//     public async Task<ResponseDto> DeleteAdminActionAsync(int actionId)
//     {
//         await using var connection = await _connectionService.GetDatabaseConnectionAsync();
//
//         // SQL query to delete an admin action by ID
//         var sql = """
//               DELETE FROM AdminAction 
//               WHERE ActionId = @ActionId
//               """;
//
//         var parameters = new DynamicParameters();
//         parameters.Add("@ActionId", actionId, DbType.Int32); // Corrected parameter name
//
//         var affectedRows = await connection.ExecuteAsync(sql, parameters).ConfigureAwait(false);
//
//         if (affectedRows <= 0)
//         {
//             return new ResponseDto
//             {
//                 IsSuccess = false,
//                 Message = "Failed to delete admin action."
//             };
//         }
//
//         return new ResponseDto
//         {
//             IsSuccess = true,
//             Message = "Admins action deleted successfully."
//         };
//     }
// }
