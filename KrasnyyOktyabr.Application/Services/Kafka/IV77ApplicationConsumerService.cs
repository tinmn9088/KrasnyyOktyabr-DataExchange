using static KrasnyyOktyabr.Application.Services.Kafka.IV77ApplicationConsumerService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV77ApplicationConsumerService : IRestartableHostedService<List<V77ApplicationConsumerStatus>>, IAsyncDisposable
{
    public readonly struct V77ApplicationConsumerStatus
    {
        public required bool Active { get; init; }

        public required DateTimeOffset LastActivity { get; init; }

        public required string? ErrorMessage { get; init; }

        public required string InfobaseName { get; init; }

        public required int Consumed { get; init; }

        public required int Saved { get; init; }

        public required IReadOnlyList<string> Topics { get; init; }

        public required string ConsumerGroup { get; init; }
    }
}
