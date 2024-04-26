using System;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public interface IV77ApplicationConsumerService : IRestartableHostedService<IStatusContainer<V77ApplicationConsumerStatus>>, IAsyncDisposable
{
}
