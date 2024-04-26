using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using Microsoft.Extensions.Hosting;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public interface IRestartableHostedService : IHostedService, IRestartable
{
    int ManagedInstancesCount { get; }
}

/// <summary>
/// <see cref="IRestartableHostedService"/> with <see cref="Status"/> property.
/// </summary>
public interface IRestartableHostedService<out TStatus> : IRestartableHostedService where TStatus : IStatusContainer<AbstractStatus>
{
    TStatus Status { get; }
}
