using KrasnyyOktyabr.Application.Contracts;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using KrasnyyOktyabr.Application.Services.Kafka;
using static KrasnyyOktyabr.Application.Logging.RestartServiceLoggingHelper;

namespace KrasnyyOktyabr.Application.Services;

public class RestartService(ILogger<RestartService> logger, IServiceProvider provider) : IRestartService
{
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(3));

    /// <summary>
    /// Minimout timeout before inactive service has to be restarted.
    /// </summary>
    public static TimeSpan MinRestartTimeout => TimeSpan.FromMinutes(15);

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.Starting();

        try
        {
            while (await _timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                logger.CheckingStatus();

                await CheckHealthAndRestart<IV77ApplicationProducerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IV77ApplicationConsumerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IMsSqlConsumerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IV83ApplicationProducerService>(provider, logger, cancellationToken).ConfigureAwait(false);

                await CheckHealthAndRestart<IV83ApplicationConsumerService>(provider, logger, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            logger.OperationCancelled();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.Stopping();

        _timer.Dispose();

        logger.Stopped();

        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        logger.Disposing();

        _timer.Dispose();

        logger.Disposed();

        return ValueTask.CompletedTask;
    }

    public async ValueTask<RestartResult> RestartAsync(CancellationToken cancellationToken)
    {
        logger.Restarting();

        (int producers1C7Stopped, int producers1C7Started) = await Restart<IV77ApplicationProducerService>(provider, cancellationToken).ConfigureAwait(false);

        (int consumers1C7Stopped, int consumers1C7Started) = await Restart<IV77ApplicationConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        (int consumersMsSqlStopped, int consumersMsSqlStarted) = await Restart<IMsSqlConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        (int producers1C8Stopped, int producers1C8Started) = await Restart<IV83ApplicationProducerService>(provider, cancellationToken).ConfigureAwait(false);

        (int consumers1C8Stopped, int consumers1C8Started) = await Restart<IV83ApplicationConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        int consumerInstructionCleared = provider.GetRequiredService<IJsonService>().ClearCachedExpressions();

        provider.GetRequiredService<IKafkaService>().Restart();

        logger.Restarted();

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
        T? service = provider.GetService<T>();

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
                    logger.InactiveFound(service.GetType().Name, status.ServiceKey);

                    DateTimeOffset restartAfter = status.LastActivity + MinRestartTimeout;

                    if (DateTimeOffset.Now < restartAfter)
                    {
                        logger.MinRestartTimeoutNotExpired(service.GetType().Name, status.ServiceKey, restartAfter);

                        continue;
                    }

                    await service.RestartAsync(status.ServiceKey, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
