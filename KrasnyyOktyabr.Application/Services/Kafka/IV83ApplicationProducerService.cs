using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV83ApplicationProducerService : IRestartableHostedService<IStatusContainer<V83ApplicationProducerStatus>>, IAsyncDisposable
{
}
