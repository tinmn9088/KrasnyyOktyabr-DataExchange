using System.Runtime.Versioning;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.Application.Services.DataResolve;

[SupportedOSPlatform("windows")]
public class ComV77ApplicationResolver : IDataResolver
{
    private readonly IComV77ApplicationConnectionFactory _connectionFactory;

    private readonly ConnectionProperties _connectionProperties;

    private readonly string _ertRelativePath;

    private readonly Dictionary<string, object?>? _context;

    private readonly string? _resultName;

    public ComV77ApplicationResolver(
        IComV77ApplicationConnectionFactory connectionFactory,
        ConnectionProperties connectionProperties,
        string ertRelativePath,
        Dictionary<string, object?>? context,
        string? resultName)
    {
        ArgumentNullException.ThrowIfNull(connectionFactory);
        ArgumentNullException.ThrowIfNull(ertRelativePath);

        _connectionFactory = connectionFactory;
        _connectionProperties = connectionProperties;
        _ertRelativePath = ertRelativePath;
        _context = context;
        _resultName = resultName;
    }

    public async ValueTask<object?> ResolveAsync(CancellationToken cancellationToken)
    {
        await using IComV77ApplicationConnection connection = await _connectionFactory.GetConnectionAsync(_connectionProperties, cancellationToken).ConfigureAwait(false);

        await connection.ConnectAsync(cancellationToken).ConfigureAwait(false);

        object? result = await connection.RunErtAsync(_ertRelativePath, _context, _resultName, cancellationToken);

        return result;
    }
}
