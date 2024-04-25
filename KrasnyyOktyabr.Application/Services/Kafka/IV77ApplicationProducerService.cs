using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV77ApplicationProducerService : IRestartableHostedService<IStatusContainer<V77ApplicationProducerStatus>>, IAsyncDisposable
{
}
