using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public interface IMsSqlService
{
    public enum ConnectionType
    {
        SqlConnection,
        OleDbConnection
    }

#nullable enable
    /// <returns>
    /// First column of the first row of the result set.
    /// </returns>
    Task<object?> SelectSingleValueAsync(string connectionString, string query);

    /// <returns>
    /// First column of the first row of the result set.
    /// </returns>
    Task<object?> SelectSingleValueAsync(string connectionString, string query, ConnectionType connectionType);
#nullable disable

    Task InsertAsync(string connectionString, string table, Dictionary<string, dynamic> columnsValues);

    Task InsertAsync(string connectionString, string table, Dictionary<string, dynamic> columnsValues, ConnectionType connectionType);
}
