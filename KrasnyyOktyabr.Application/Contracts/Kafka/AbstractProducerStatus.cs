using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public abstract class AbstractProducerStatus : AbstractStatus
{
    [JsonPropertyName("transactionTypeFilters")]
    public required IReadOnlyList<string> TransactionTypeFilters { get; init; }

    [JsonPropertyName("produced")]
    public required int Produced { get; init; }

    [JsonPropertyName("dataTypePropertyName")]
    public required string DataTypePropertyName { get; init; }
}
