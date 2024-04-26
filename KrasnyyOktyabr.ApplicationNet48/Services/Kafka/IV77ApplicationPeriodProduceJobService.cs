using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public interface IV77ApplicationPeriodProduceJobService : IAsyncDisposable
{
    List<V77ApplicationPeriodProduceJobStatus> Status { get; }

    void StartJob(V77ApplicationPeriodProduceJobRequest request);

    ValueTask CancelJobAsync(string infobasePath);
}
