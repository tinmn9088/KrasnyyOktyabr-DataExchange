namespace KrasnyyOktyabr.ComV77Application;

/// <summary>
/// Represents connection to <c>V77.Application</c> COM object.
/// </summary>
public interface IComV77ApplicationConnection : IAsyncDisposable
{
    public readonly struct ComV77ApplicationConnectionStatus
    {
        public ComV77ApplicationConnectionStatus(
            string infobasePath,
            string username,
            DateTimeOffset? lastTimeDisposed,
            int errorsCount,
            int retrievedTimes,
            bool isInitialized,
            bool isDisposed)
        {
            InfobasePath = infobasePath;
            Username = username;
            LastTimeDisposed = lastTimeDisposed;
            ErrorsCount = errorsCount;
            RetrievedTimes = retrievedTimes;
            IsInitialized = isInitialized;
            IsDisposed = isDisposed;
        }

        public string InfobasePath { get; }

        public string Username { get; }

        public DateTimeOffset? LastTimeDisposed { get; }

        public int ErrorsCount { get; }

        public int RetrievedTimes { get; }

        public bool IsInitialized { get; }

        public bool IsDisposed { get; }
    }

    ComV77ApplicationConnectionStatus Status { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task<object?> RunErtAsync(string ertRelativePath, Dictionary<string, object?>? ertContext, string? resultName, CancellationToken cancellationToken);
}
