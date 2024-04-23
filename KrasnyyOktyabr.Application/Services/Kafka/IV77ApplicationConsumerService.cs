using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV77ApplicationConsumerService : IRestartableHostedService<List<V77ApplicationConsumerStatus>>, IAsyncDisposable
{
}
