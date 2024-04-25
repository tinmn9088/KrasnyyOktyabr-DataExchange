using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class AbstractConsumerStatus : AbstractStatus
{
    [JsonPropertyName("consumed")]
    public required int Consumed { get; init; }

    [JsonPropertyName("topics")]
    public required IReadOnlyList<string> Topics { get; init; }

    [JsonPropertyName("consumerGroup")]
    public required string ConsumerGroup { get; init; }
}
