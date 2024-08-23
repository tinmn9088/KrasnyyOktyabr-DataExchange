using System;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.CoreServices.ProducerServices;

public interface IV83ApplicationProducerService : IRestartableHostedService<IStatusContainer<V83ApplicationProducerStatus>>, IAsyncDisposable
{
}
