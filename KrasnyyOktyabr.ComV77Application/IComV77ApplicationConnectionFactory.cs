using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using static KrasnyyOktyabr.ComV77Application.IComV77ApplicationConnection;

namespace KrasnyyOktyabr.ComV77Application;

public interface IComV77ApplicationConnectionFactory : IAsyncDisposable
{
    public readonly struct ComV77ApplicationConnectionFactoryStatus(ComV77ApplicationConnectionStatus[] connections)
    {
        public ComV77ApplicationConnectionStatus[] Connections { get; } = connections;
    }

    ComV77ApplicationConnectionFactoryStatus Status { get; }

    Task<IComV77ApplicationConnection> GetConnectionAsync(ConnectionProperties connectionProperties, CancellationToken cancellationToken = default);
}
