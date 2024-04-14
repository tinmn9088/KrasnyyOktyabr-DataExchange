using System.Runtime.Versioning;

namespace KrasnyyOktyabr.ComV77Application;

/// <summary>
/// Represents connection to <c>V77.Application</c> COM object.
/// </summary>
[SupportedOSPlatform("windows")]
public interface IComV77ApplicationConnection : IAsyncDisposable
{
    Task ConnectAsync(CancellationToken cancellationToken);

    Task<object?> RunErtAsync(string ertRelativePath, Dictionary<string, object?>? ertContext, string? resultName, CancellationToken cancellationToken);
}
