using System;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.ApplicationNet48.Services.DataResolve;

/// <exception cref="ArgumentNullException"></exception>
public class MsSqlSingleValueDataResolver(IMsSqlService service, string connectionString, string query) : IDataResolver
{
    private readonly IMsSqlService _service = service ?? throw new ArgumentNullException(nameof(service));

    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    private readonly string _query = query ?? throw new ArgumentNullException(nameof(query));

#nullable enable
    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        object? value = await _service.SelectSingleValueAsync(_connectionString, _query);
        return value;
    }
}
