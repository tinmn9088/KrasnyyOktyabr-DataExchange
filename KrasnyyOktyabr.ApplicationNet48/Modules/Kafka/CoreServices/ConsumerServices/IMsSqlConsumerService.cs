using System;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.CoreServices.ConsumerServices;

public interface IMsSqlConsumerService : IRestartableHostedService<IStatusContainer<MsSqlConsumerStatus>>, IAsyncDisposable
{
}
