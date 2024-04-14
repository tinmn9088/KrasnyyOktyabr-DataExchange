using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;

namespace KrasnyyOktyabr.ComV77Application;

public interface IComV77ApplicationConnectionFactory<TOut> : IAsyncDisposable where TOut : IComV77ApplicationConnection
{
    Task<TOut> GetConnectionAsync(ConnectionProperties connectionProperties);
}
