using System;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Models;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public class RestartService(ILogger<RestartService> logger, IServiceProvider provider) : IRestartService
{
    private static TimeSpan CheckInterval => TimeSpan.FromMinutes(3);

    /// <summary>
    /// Minimout timeout before inactive service has to be restarted.
    /// </summary>
    public static TimeSpan MinRestartTimeout => TimeSpan.FromHours(1);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Starting ...");

        try
        {
            while (true)
            {
                await Task.Delay(CheckInterval).ConfigureAwait(false);

                logger.LogTrace("Checking status");

                await CheckHealthAndRestart<IV77ApplicationProducerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IV77ApplicationConsumerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IMsSqlConsumerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IV83ApplicationProducerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IV83ApplicationConsumerService>(provider, logger, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Operation cancelled");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Stopped");

        return Task.CompletedTask;
    }

    public async ValueTask<RestartResult> RestartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Restaring all");

        (int producers1C7Stopped, int producers1C7Started) = await Restart<IV77ApplicationProducerService>(provider, cancellationToken).ConfigureAwait(false);

        (int consumers1C7Stopped, int consumers1C7Started) = await Restart<IV77ApplicationConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        (int consumersMsSqlStopped, int consumersMsSqlStarted) = await Restart<IMsSqlConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        (int producers1C8Stopped, int producers1C8Started) = await Restart<IV83ApplicationProducerService>(provider, cancellationToken).ConfigureAwait(false);

        (int consumers1C8Stopped, int consumers1C8Started) = await Restart<IV83ApplicationConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        int consumerInstructionCleared = provider.GetRequiredService<IJsonService>().ClearCachedExpressions();

        provider.GetRequiredService<IKafkaService>().Restart();

        logger.LogTrace("Restarted all");

        return new RestartResult
        {
            Producers1C7Stopped = producers1C7Stopped,
            Producers1C7Started = producers1C7Started,

            Producers1C8Stopped = producers1C8Stopped,
            Producers1C8Started = producers1C8Started,

            Consumers1C7Stopped = consumers1C7Stopped,
            Consumers1C7Started = consumers1C7Started,

            Consumers1C8Stopped = consumers1C8Stopped,
            Consumers1C8Started = consumers1C8Started,

            ConsumersMsSqlStopped = consumersMsSqlStopped,
            ConsumersMsSqlStarted = consumersMsSqlStarted,

            ConsumerInstructionsCleared = consumerInstructionCleared,
        };
    }

    private static async ValueTask<(int, int)> Restart<T>(IServiceProvider provider, CancellationToken cancellationToken) where T : IRestartableHostedService
    {
        T? service = provider.GetService<T>();

        int stopped = 0;
        int started = 0;

        if (service != null)
        {
            stopped = service.ManagedInstancesCount;

            await service.RestartAsync(cancellationToken).ConfigureAwait(false);

            started = service.ManagedInstancesCount;
        }

        return (stopped, started);
    }

    private static async ValueTask CheckHealthAndRestart<T>(IServiceProvider provider, ILogger logger, CancellationToken cancellationToken)
        where T : IRestartableHostedService<IStatusContainer<AbstractStatus>>
    {
#nullable enable
        T? service = provider.GetService<T>();
#nullable disable

        if (service == null)
        {
            return;
        }

        IStatusContainer<AbstractStatus> serviceStatus = service.Status;

        if (serviceStatus.Statuses != null)
        {
            foreach (AbstractStatus status in serviceStatus.Statuses)
            {
                if (!status.Active)
                {
                    logger.LogWarning("Found inactive '{TypeName}', key '{Key}'", service.GetType().Name, status.ServiceKey);

                    DateTimeOffset restartAfter = status.LastActivity + MinRestartTimeout;

                    if (DateTimeOffset.Now < restartAfter)
                    {
                        logger.LogTrace("Minimal timeout not expired yet for '{TypeName}', key '{Key}' (can be restarted after {RestartAfter})", service.GetType().Name, status.ServiceKey, restartAfter);

                        continue;
                    }

                    await service.RestartAsync(status.ServiceKey, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
