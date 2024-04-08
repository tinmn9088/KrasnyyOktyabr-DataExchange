namespace KrasnyyOktyabr.ComV77ApplicationConnection;

public interface IComV77ApplicationConnectionFactory<TOut> : IAsyncDisposable where TOut : IComV77ApplicationConnection
{
    Task<TOut> GetConnectionAsync(string infobasePath, string username, string password);
}
