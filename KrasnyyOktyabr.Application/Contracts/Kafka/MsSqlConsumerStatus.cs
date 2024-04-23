using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class MsSqlConsumerStatus : AbstractConsumerStatus
{
    [JsonPropertyName("saved")]
    public required int Saved { get; init; }

    [JsonPropertyName("tablePropertyName")]
    public required string TablePropertyName { get; init; }
}
