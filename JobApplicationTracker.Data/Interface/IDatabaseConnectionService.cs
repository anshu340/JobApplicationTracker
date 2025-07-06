using Microsoft.Data.SqlClient;

namespace JobApplicationTracker.Data.Interface;
    public interface IDatabaseConnectionService
    {
        Task<SqlConnection> GetDatabaseConnectionAsync();
        Task CloseDatabaseConnectionAsync(SqlConnection connection);
    }
