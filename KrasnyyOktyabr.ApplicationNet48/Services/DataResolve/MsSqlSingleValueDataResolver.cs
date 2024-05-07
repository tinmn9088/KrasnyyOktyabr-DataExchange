#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;
using static KrasnyyOktyabr.ApplicationNet48.Services.IMsSqlService;

namespace KrasnyyOktyabr.ApplicationNet48.Services.DataResolve;

/// <exception cref="ArgumentNullException"></exception>
public class MsSqlSingleValueDataResolver(IMsSqlService service, string connectionString, string query, ConnectionType? connectionType) : IDataResolver
{
    private readonly IMsSqlService _service = service ?? throw new ArgumentNullException(nameof(service));

    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    private readonly string _query = query ?? throw new ArgumentNullException(nameof(query));

    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        return connectionType != null
            ? await _service.SelectSingleValueAsync(_connectionString, _query, connectionType.Value)
            : await _service.SelectSingleValueAsync(_connectionString, _query);
    }
}
