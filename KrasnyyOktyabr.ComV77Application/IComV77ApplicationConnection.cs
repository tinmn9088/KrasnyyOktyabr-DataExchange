using static KrasnyyOktyabr.ComV77Application.Helpers.LoggingHelper;

namespace KrasnyyOktyabr.ComV77Application;

/// <summary>
/// Represents connection to <c>V77.Application</c> COM object.
/// </summary>
public interface IComV77ApplicationConnection : IAsyncDisposable
{
    public readonly struct ComV77ApplicationConnectionStatus(
        string infobasePath,
        string username,
        DateTimeOffset? lastTimeDisposed,
        int errorsCount,
        int retrievedTimes,
        bool isInitialized,
        bool isDisposed)
    {
        public string InfobasePath { get; } = infobasePath;

        public string Username { get; } = username;

        public DateTimeOffset? LastTimeDisposed { get; } = lastTimeDisposed;

        public int ErrorsCount { get; } = errorsCount;

        public int RetrievedTimes { get; } = retrievedTimes;

        public bool IsInitialized { get; } = isInitialized;

        public bool IsDisposed { get; } = isDisposed;
    }

    ComV77ApplicationConnectionStatus Status { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="FailedToInvokeMemberException"></exception>
    /// <exception cref="FailedToRunErtException"></exception>
    Task<object?> RunErtAsync(string ertRelativePath, IReadOnlyDictionary<string, string>? ertContext, CancellationToken cancellationToken);


    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="FailedToInvokeMemberException"></exception>
    /// <exception cref="FailedToRunErtException"></exception>
    Task<object?> RunErtAsync(string ertRelativePath, IReadOnlyDictionary<string, string>? ertContext, string? resultName, CancellationToken cancellationToken);

    /// <exception cref="OperationCanceledException"></exception>
    /// <exception cref="FailedToInvokeMemberException"></exception>
    /// <exception cref="FailedToRunErtException"></exception>
    Task<object?> RunErtAsync(string ertRelativePath, IReadOnlyDictionary<string, string>? ertContext, string? resultName, string? errorMessageName, CancellationToken cancellationToken);

    public class FailedToInvokeMemberException(string memberName, object?[]? args, Exception innerException)
        : Exception($"Failed to invoke member '{memberName}' with args: {BuildArgsString(args)}", innerException)
    {
    }

    public class FailedToRunErtException(string errorMessage)
        : Exception($"Failed to run ERT: {errorMessage}")
    {
    }
}
