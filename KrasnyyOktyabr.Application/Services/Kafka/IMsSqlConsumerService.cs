using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IMsSqlConsumerService : IRestartableHostedService<IStatusContainer<MsSqlConsumerStatus>>, IAsyncDisposable
{
}
