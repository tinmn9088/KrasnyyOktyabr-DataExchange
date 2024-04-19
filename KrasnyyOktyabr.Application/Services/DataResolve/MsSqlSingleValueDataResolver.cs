using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.Application.Services.DataResolve;

public class MsSqlSingleValueDataResolver : IDataResolver
{
    private readonly IMsSqlService _service;

    private readonly string _connectionString;

    private readonly string _query;

    /// <exception cref="ArgumentNullException"></exception>
    public MsSqlSingleValueDataResolver(IMsSqlService service, string connectionString, string query)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(connectionString);
        ArgumentNullException.ThrowIfNull(query);

        _service = service;
        _connectionString = connectionString;
        _query = query;
    }

    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        object? value = await _service.SelectSingleValueAsync(_connectionString, _query);
        return value;
    }
}
