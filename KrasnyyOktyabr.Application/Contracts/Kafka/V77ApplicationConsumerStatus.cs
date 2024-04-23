using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class V77ApplicationConsumerStatus : AbstractConsumerStatus
{
    [JsonPropertyName("infobasePath")]
    public required string InfobaseName { get; init; }

    [JsonPropertyName("saved")]
    public required int Saved { get; init; }
}
