namespace KrasnyyOktyabr.Application.Services;

public interface IRestartable
{
    Task RestartAsync(CancellationToken cancellationToken);
}
