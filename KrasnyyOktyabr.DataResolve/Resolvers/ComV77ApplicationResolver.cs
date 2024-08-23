#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;

namespace KrasnyyOktyabr.DataResolve.Resolvers;

public class ComV77ApplicationResolver(
    IComV77ApplicationConnectionFactory connectionFactory,
    ConnectionProperties connectionProperties,
    string ertRelativePath,
    IReadOnlyDictionary<string, string>? context,
    string? resultName,
    string? errorMessageName)
    : IDataResolver
{
    private readonly IComV77ApplicationConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    private readonly string _ertRelativePath = ertRelativePath ?? throw new ArgumentNullException(nameof(ertRelativePath));

    public ComV77ApplicationResolver(
        IComV77ApplicationConnectionFactory connectionFactory,
        ConnectionProperties connectionProperties,
        string ertRelativePath,
        IReadOnlyDictionary<string, string>? context,
        string? resultName)
        : this(connectionFactory, connectionProperties, ertRelativePath, context, resultName, errorMessageName: "Error")
    {
    }

    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        await using IComV77ApplicationConnection connection = await _connectionFactory.GetConnectionAsync(connectionProperties, cancellationToken).ConfigureAwait(false);

        await connection.ConnectAsync(cancellationToken).ConfigureAwait(false);

        object? result = await connection.RunErtAsync(_ertRelativePath, context, resultName, errorMessageName, cancellationToken);

        return result;
    }
}
