using System;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public interface IV77ApplicationProducerService : IRestartableHostedService<IStatusContainer<V77ApplicationProducerStatus>>, IAsyncDisposable
{
}
