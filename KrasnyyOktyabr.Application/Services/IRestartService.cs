using KrasnyyOktyabr.Application.Contracts;

namespace KrasnyyOktyabr.Application.Services;

public interface IRestartService : IHostedService, IAsyncDisposable
{
    ValueTask<RestartResult> RestartAsync(CancellationToken cancellationToken);
}
