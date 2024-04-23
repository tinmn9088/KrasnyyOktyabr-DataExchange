using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV83ApplicationConsumerService : IRestartableHostedService<List<V83ApplicationConsumerStatus>>, IAsyncDisposable
{
}
