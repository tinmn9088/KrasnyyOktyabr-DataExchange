using static KrasnyyOktyabr.Application.Services.Kafka.V77ApplicationProducerService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV77ApplicationProducerService : IRestartableHostedService, IAsyncDisposable
{
    public readonly struct V77ApplicationProducerStatus
    {
        public required bool Active { get; init; }

        public required DateTimeOffset LastActivity { get; init; }

        public required string? ErrorMessage { get; init; }

        public required IReadOnlyList<ObjectFilter> ObjectFilters { get; init; }

        public required IReadOnlyList<string> TransactionTypes { get; init; }

        public required int GotLogTransactions { get; init; }

        public required int Fetched { get; init; }

        public required int Produced { get; init; }

        public required string Username { get; init; }

        public required string InfobasePath { get; init; }

        public required string DataTypeJsonPropertyName { get; init; }
    }

    List<V77ApplicationProducerStatus> GetStatus();
}
