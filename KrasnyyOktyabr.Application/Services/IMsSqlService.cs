namespace KrasnyyOktyabr.Application.Services;

public interface IMsSqlService
{
    /// <returns>
    /// First column of the first row of the result set.
    /// </returns>
    Task<object?> SelectSingleValueAsync(string connectionString, string query);

    Task InsertAsync(string connectionString, string table, Dictionary<string, dynamic> columnsValues);
}
