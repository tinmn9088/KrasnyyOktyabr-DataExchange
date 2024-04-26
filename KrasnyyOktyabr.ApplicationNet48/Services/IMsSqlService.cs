using System.Collections.Generic;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public interface IMsSqlService
{
#nullable enable
    /// <returns>
    /// First column of the first row of the result set.
    /// </returns>
    Task<object?> SelectSingleValueAsync(string connectionString, string query);
#nullable disable

    Task InsertAsync(string connectionString, string table, Dictionary<string, dynamic> columnsValues);
}
