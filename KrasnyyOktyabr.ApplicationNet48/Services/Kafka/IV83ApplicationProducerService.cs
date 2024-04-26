using System;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public interface IV83ApplicationProducerService : IRestartableHostedService<IStatusContainer<V83ApplicationProducerStatus>>, IAsyncDisposable
{
}
