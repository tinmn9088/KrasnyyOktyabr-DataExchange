#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using MsSql;

namespace KrasnyyOktyabr.DataResolve.Resolvers;

/// <exception cref="ArgumentNullException"></exception>
public class MsSqlSingleValueDataResolver(IMsSqlService service, string connectionString, string query, IMsSqlService.ConnectionType? connectionType) : IDataResolver
{
    private readonly IMsSqlService _service = service ?? throw new ArgumentNullException(nameof(service));

    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

    private readonly string _query = query ?? throw new ArgumentNullException(nameof(query));

    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        return connectionType is not null
            ? await _service.SelectSingleValueAsync(_connectionString, _query, connectionType.Value)
            : await _service.SelectSingleValueAsync(_connectionString, _query);
    }
}
