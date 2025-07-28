using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace JobApplicationTracker.Data.Interface
{
    public interface IDatabaseConnectionService
    {
        Task<SqlConnection> GetDatabaseConnectionAsync();
        Task CloseDatabaseConnectionAsync(SqlConnection connection);
    }
}
