namespace KrasnyyOktyabr.Application.Services;

public interface IRestartable
{
    ValueTask RestartAsync(CancellationToken cancellationToken);

    ValueTask RestartAsync(string key, CancellationToken cancellationToken);
}
