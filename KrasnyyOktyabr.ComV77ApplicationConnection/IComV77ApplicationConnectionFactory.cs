using KrasnyyOktyabr.ComV77ApplicationConnection.Contracts.Configuration;

namespace KrasnyyOktyabr.ComV77ApplicationConnection;

public interface IComV77ApplicationConnectionFactory<TOut> : IAsyncDisposable where TOut : IComV77ApplicationConnection
{
    Task<TOut> GetConnectionAsync(ConnectionProperties connectionProperties);
}
