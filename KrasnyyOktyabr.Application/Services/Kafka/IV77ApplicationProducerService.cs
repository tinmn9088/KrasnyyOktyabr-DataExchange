using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV77ApplicationProducerService : IRestartableHostedService<List<V77ApplicationProducerStatus>>, IAsyncDisposable
{
}
