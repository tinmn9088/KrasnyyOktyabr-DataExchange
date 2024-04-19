namespace KrasnyyOktyabr.Application.Services;

public interface IRestartable
{
    ValueTask RestartAsync(CancellationToken cancellationToken);
}
