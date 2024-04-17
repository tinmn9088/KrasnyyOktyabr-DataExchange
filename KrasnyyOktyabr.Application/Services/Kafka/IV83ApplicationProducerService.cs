using static KrasnyyOktyabr.Application.Services.Kafka.IV83ApplicationProducerService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV83ApplicationProducerService : IRestartableHostedService<List<V83ApplicationProducerStatus>>, IAsyncDisposable
{
    public readonly struct V83ApplicationProducerStatus
    {
        public required bool Active { get; init; }

        public required DateTimeOffset LastActivity { get; init; }

        public required string? ErrorMessage { get; init; }

        public required IReadOnlyList<string> ObjectFilters { get; init; }

        public required IReadOnlyList<string> TransactionTypes { get; init; }

        public required int Fetched { get; init; }

        public required int Produced { get; init; }

        public required string Username { get; init; }

        public required string InfobasePath { get; init; }

        public required string DataTypeJsonPropertyName { get; init; }
    }
}
