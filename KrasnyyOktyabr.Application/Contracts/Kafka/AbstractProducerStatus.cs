using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public abstract class AbstractProducerStatus
{
    [JsonPropertyName("active")]
    public required bool Active { get; init; }

    [JsonPropertyName("lastActivity")]
    public required DateTimeOffset LastActivity { get; init; }

    [JsonPropertyName("errorMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? ErrorMessage { get; init; }

    [JsonPropertyName("transactionTypes")]
    public required IReadOnlyList<string> TransactionTypes { get; init; }

    [JsonPropertyName("produced")]
    public required int Produced { get; init; }

    [JsonPropertyName("dataTypePropertyName")]
    public required string DataTypePropertyName { get; init; }
}
