using static KrasnyyOktyabr.Application.Services.Kafka.IMsSqlConsumerService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IMsSqlConsumerService : IRestartableHostedService<List<MsSqlProducerStatus>>, IAsyncDisposable
{
    public readonly struct MsSqlProducerStatus
    {
        public required bool Active { get; init; }

        public required DateTimeOffset LastActivity { get; init; }

        public required string? ErrorMessage { get; init; }

        public required int Consumed { get; init; }

        public required int Saved { get; init; }

        public required IReadOnlyList<string> Topics { get; init; }

        public required string TablePropertyName { get; init; }

        public required string ConsumerGroup { get; init; }
    }
}
