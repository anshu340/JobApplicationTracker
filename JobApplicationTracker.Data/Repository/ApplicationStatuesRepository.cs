using Dapper;
using System.Data;
using JobApplicationTracker.Data.DataModels;
using JobApplicationTracker.Data.Dto;
using JobApplicationTracker.Data.Dtos.Responses;
using JobApplicationTracker.Data.Interface;

namespace JobApplicationTracker.Data.Repository
{
    public class ApplicationStatusRepository : IApplicationStatusRepository
    {
        private readonly IDatabaseConnectionService _connectionService;

        public ApplicationStatusRepository(IDatabaseConnectionService connectionService)
        {
            _connectionService = connectionService;
        }

        public async Task<IEnumerable<ApplicationStatusDto>> GetAllApplicationStatusesAsync()
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = """
                      SELECT ApplicationStatusId,
                             StatusName,
                             Description
                      FROM ApplicationStatus
                      """;

            return await connection.QueryAsync<ApplicationStatusDto>(sql).ConfigureAwait(false);
        }

        public async Task<ApplicationStatusDto?> GetApplicationStatusByIdAsync(int id)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = """
                      SELECT ApplicationStatusId,
                             StatusName,
                             Description
                      FROM ApplicationStatus
                      WHERE ApplicationStatusId = @ApplicationStatusId
                      """;

            var parameters = new DynamicParameters();
            parameters.Add("@ApplicationStatusId", id, DbType.Int32);

            return await connection.QueryFirstOrDefaultAsync<ApplicationStatusDto>(sql, parameters).ConfigureAwait(false);
        }

        public async Task<ApplicationStatusDto> CreateApplicationStatusAsync(ApplicationStatusDto applicationStatus)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            // If ID is less than or equal to 0, perform INSERT
            if (applicationStatus.ApplicationStatusId <= 0)
            {
                var insertSql = """
                                INSERT INTO ApplicationStatus (
                                    StatusName, Description
                                )
                                VALUES (
                                    @StatusName, @Description
                                );
                                SELECT CAST(SCOPE_IDENTITY() AS INT);
                                """;

                var insertParameters = new DynamicParameters();
                insertParameters.Add("@StatusName", applicationStatus.StatusName, DbType.String);
                insertParameters.Add("@Description", applicationStatus.Description, DbType.String);

                var newId = await connection.QuerySingleAsync<int>(insertSql, insertParameters).ConfigureAwait(false);
                applicationStatus.ApplicationStatusId = newId;

                return applicationStatus;
            }
            // If ID is greater than 0, perform UPDATE
            else
            {
                var updateSql = """
                                UPDATE ApplicationStatus
                                SET 
                                    StatusName = @StatusName,
                                    Description = @Description
                                WHERE ApplicationStatusId = @ApplicationStatusId
                                """;

                var updateParameters = new DynamicParameters();
                updateParameters.Add("@ApplicationStatusId", applicationStatus.ApplicationStatusId, DbType.Int32);
                updateParameters.Add("@StatusName", applicationStatus.StatusName, DbType.String);
                updateParameters.Add("@Description", applicationStatus.Description, DbType.String);

                var affectedRows = await connection.ExecuteAsync(updateSql, updateParameters).ConfigureAwait(false);

                // If no rows were affected, the record doesn't exist
                if (affectedRows <= 0)
                {
                    throw new InvalidOperationException($"Application status with ID {applicationStatus.ApplicationStatusId} not found for update.");
                }

                return applicationStatus;
            }
        }
        public async Task<bool> DeleteApplicationStatusAsync(int id)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            // Check referential integrity first (example for JobApplications table)
            var refCheckSql = "SELECT COUNT(1) FROM JobApplications WHERE ApplicationStatusId = @ApplicationStatusId";
            var hasDependencies = await connection.ExecuteScalarAsync<int>(refCheckSql, new { ApplicationStatusId = id });

            if (hasDependencies > 0)
            {
                return false; // Cannot delete due to dependencies
            }

            // Delete the application status
            var deleteSql = "DELETE FROM ApplicationStatus WHERE ApplicationStatusId = @ApplicationStatusId";

            var parameters = new DynamicParameters();
            parameters.Add("@ApplicationStatusId", id, DbType.Int32);

            var affectedRows = await connection.ExecuteAsync(deleteSql, parameters).ConfigureAwait(false);

            return affectedRows > 0;
        }

        public async Task<bool> ApplicationStatusExistsAsync(int id)
        {
            await using var connection = await _connectionService.GetDatabaseConnectionAsync();

            var sql = """
                      SELECT 1 
                      FROM ApplicationStatus 
                      WHERE ApplicationStatusId = @ApplicationStatusId
                      """;

            var parameters = new DynamicParameters();
            parameters.Add("@ApplicationStatusId", id, DbType.Int32);

            var result = await connection.ExecuteScalarAsync<int?>(sql, parameters).ConfigureAwait(false);
            return result.HasValue;
        }

    }
}