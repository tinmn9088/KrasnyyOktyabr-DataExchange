using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class V83ApplicationConsumerStatus : AbstractConsumerStatus
{
    [JsonPropertyName("infobaseName")]
    public required string InfobaseName { get; init; }

    [JsonPropertyName("saved")]
    public required int Saved { get; init; }
}
