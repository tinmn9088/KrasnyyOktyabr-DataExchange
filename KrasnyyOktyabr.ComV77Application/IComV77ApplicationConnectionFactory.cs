using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;

namespace KrasnyyOktyabr.ComV77Application;

public interface IComV77ApplicationConnectionFactory : IAsyncDisposable
{
    Task<IComV77ApplicationConnection> GetConnectionAsync(ConnectionProperties connectionProperties, CancellationToken cancellationToken = default);
}
