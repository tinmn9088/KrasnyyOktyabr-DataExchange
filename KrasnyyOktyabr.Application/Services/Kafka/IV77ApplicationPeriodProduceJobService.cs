using KrasnyyOktyabr.Application.Contracts.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV77ApplicationPeriodProduceJobService : IAsyncDisposable
{
    List<V77ApplicationPeriodProduceJobStatus> Status { get; }

    void StartJob(V77ApplicationPeriodProduceJobRequest request);

    ValueTask CancelJobAsync(string infobasePath);
}
