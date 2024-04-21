using System.Runtime.InteropServices;
using KrasnyyOktyabr.Application.Contracts;
using KrasnyyOktyabr.Application.Services.Kafka;

namespace KrasnyyOktyabr.Application.Services;

public class RestartService : IRestartService
{
    public async ValueTask<RestartResult> RestartAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        int producers1C7Stopped = 0;
        int producers1C7Started = 0;

        int consumers1C7Stopped = 0;
        int consumers1C7Started = 0;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            (producers1C7Stopped, producers1C7Started) = await Restart<IV77ApplicationProducerService>(provider, cancellationToken).ConfigureAwait(false);

            (consumers1C7Stopped, consumers1C7Started) = await Restart<IV77ApplicationConsumerService>(provider, cancellationToken).ConfigureAwait(false);
        }

        (int consumersMsSqlStopped, int consumersMsSqlStarted) = await Restart<IMsSqlConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        (int producers1C8Stopped, int producers1C8Started) = await Restart<IV83ApplicationProducerService>(provider, cancellationToken).ConfigureAwait(false);

        (int consumers1C8Stopped, int consumers1C8Started) = await Restart<IV83ApplicationConsumerService>(provider, cancellationToken).ConfigureAwait(false);

        int consumerInstructionCleared = provider.GetRequiredService<IJsonService>().ClearCachedExpressions();

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
}
