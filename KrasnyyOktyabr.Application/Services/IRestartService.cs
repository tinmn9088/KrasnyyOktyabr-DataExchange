using KrasnyyOktyabr.Application.Contracts;

namespace KrasnyyOktyabr.Application.Services;

public interface IRestartService
{
    ValueTask<RestartResult> RestartAsync(IServiceProvider provider, CancellationToken cancellationToken);
}
