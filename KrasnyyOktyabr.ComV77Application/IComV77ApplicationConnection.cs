using System.Runtime.Versioning;

namespace KrasnyyOktyabr.ComV77Application;

/// <summary>
/// Represents connection to <c>V77.Application</c> COM object.
/// </summary>
[SupportedOSPlatform("windows")]
public interface IComV77ApplicationConnection : IAsyncDisposable
{
    public readonly struct ComV77ApplicationConnectionStatus
    {
        public required string InfobasePath { get; init; }

        public required string Username { get; init; }

        public required DateTimeOffset? LastTimeDisposed { get; init; }

        public required int ErrorsCount { get; init; }

        public required int RetievedTimes { get; init; }

        public required bool IsInitialized { get; init; }

        public required bool IsDisposed { get; init; }
    }

    ComV77ApplicationConnectionStatus Status { get; }

    Task ConnectAsync(CancellationToken cancellationToken);

    Task<object?> RunErtAsync(string ertRelativePath, Dictionary<string, object?>? ertContext, string? resultName, CancellationToken cancellationToken);
}
