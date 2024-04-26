using System;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public interface IMsSqlConsumerService : IRestartableHostedService<IStatusContainer<MsSqlConsumerStatus>>, IAsyncDisposable
{
}
