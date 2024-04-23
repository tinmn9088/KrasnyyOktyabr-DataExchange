using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class AbstractConsumerStatus
{
    [JsonPropertyName("active")]
    public required bool Active { get; init; }

    [JsonPropertyName("lastActivity")]
    public required DateTimeOffset LastActivity { get; init; }

    [JsonPropertyName("errorMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? ErrorMessage { get; init; }

    [JsonPropertyName("consumed")]
    public required int Consumed { get; init; }

    [JsonPropertyName("topics")]
    public required IReadOnlyList<string> Topics { get; init; }

    [JsonPropertyName("consumerGroup")]
    public required string ConsumerGroup { get; init; }
}
