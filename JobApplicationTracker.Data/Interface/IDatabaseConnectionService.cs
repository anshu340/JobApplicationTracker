using Microsoft.Data.SqlClient;

namespace JobApplicationTracke.Data.Interface;
    public interface IDatabaseConnectionService
    {
        Task<SqlConnection> GetDatabaseConnectionAsync();
        Task CloseDatabaseConnectionAsync(SqlConnection connection);
    }
