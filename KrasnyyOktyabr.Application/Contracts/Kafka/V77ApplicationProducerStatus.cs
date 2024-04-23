using System.Text.Json.Serialization;
using static KrasnyyOktyabr.Application.Services.Kafka.V77ApplicationProducerService;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class V77ApplicationProducerStatus : AbstractProducerStatus
{

    [JsonPropertyName("objectFilters")]
    public required IReadOnlyList<ObjectFilter> ObjectFilters { get; init; }

    [JsonPropertyName("gotLogTransactions")]
    public required int GotLogTransactions { get; init; }

    [JsonPropertyName("fetched")]
    public required int Fetched { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("infobasePath")]
    public required string InfobasePath { get; init; }
}
