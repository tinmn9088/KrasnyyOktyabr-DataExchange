#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.ApplicationNet48.Services.DataResolve;

public class ComV77ApplicationResolver(
    IComV77ApplicationConnectionFactory connectionFactory,
    ConnectionProperties connectionProperties,
    string ertRelativePath,
    Dictionary<string, object?>? context,
    string? resultName)
    : IDataResolver
{
    private readonly IComV77ApplicationConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    private readonly ConnectionProperties _connectionProperties = connectionProperties;

    private readonly string _ertRelativePath = ertRelativePath ?? throw new ArgumentNullException(nameof(ertRelativePath));

    private readonly Dictionary<string, object?>? _context = context;

    private readonly string? _resultName = resultName;

    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        await using IComV77ApplicationConnection connection = await _connectionFactory.GetConnectionAsync(_connectionProperties, cancellationToken).ConfigureAwait(false);

        await connection.ConnectAsync(cancellationToken).ConfigureAwait(false);

        object? result = await connection.RunErtAsync(_ertRelativePath, _context, _resultName, cancellationToken);

        return result;
    }
}
