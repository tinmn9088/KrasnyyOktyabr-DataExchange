namespace KrasnyyOktyabr.Application.Services;

public interface IRestartableHostedService : IHostedService, IRestartable
{
    int ManagedInstancesCount { get; }
}

/// <summary>
/// <see cref="IRestartableHostedService"/> with <see cref="Status"/> property.
/// </summary>
public interface IRestartableHostedService<TStatus> : IRestartableHostedService
{
    TStatus Status { get; }
}
