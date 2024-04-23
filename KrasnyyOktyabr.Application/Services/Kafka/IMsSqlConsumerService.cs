using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IMsSqlConsumerService : IRestartableHostedService<List<MsSqlConsumerStatus>>, IAsyncDisposable
{
}
