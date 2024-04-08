namespace KrasnyyOktyabr.ComV77ApplicationConnection;

public interface IComV77ApplicationConnection : IAsyncDisposable
{
    Task ConnectAsync(Properties properties, CancellationToken cancellationToken);

    public record Properties(string InfobasePath, string Username, string Password);
}
