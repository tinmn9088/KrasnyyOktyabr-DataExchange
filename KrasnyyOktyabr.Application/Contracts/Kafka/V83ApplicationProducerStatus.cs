using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class V83ApplicationProducerStatus : AbstractProducerStatus
{

    [JsonPropertyName("objectFilters")]
    public required IReadOnlyList<string> ObjectFilters { get; init; }

    [JsonPropertyName("fetched")]
    public required int Fetched { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("infobaseUrl")]
    public required string InfobaseUrl { get; init; }
}
