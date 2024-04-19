using static KrasnyyOktyabr.Application.Services.Kafka.IV83ApplicationConsumerService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IV83ApplicationConsumerService : IRestartableHostedService<List<V83ApplicationConsumerStatus>>, IAsyncDisposable
{
    public readonly struct V83ApplicationConsumerStatus
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
