using System.Collections.Generic;
using System.Text.Json.Serialization;
using static KrasnyyOktyabr.ApplicationNet48.Services.Kafka.V77ApplicationHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationProducerStatus : AbstractProducerStatus
{
    [JsonPropertyName("objectFilters")]
    public IReadOnlyList<ObjectFilter> ObjectFilters { get; set; }

    [JsonPropertyName("gotLogTransactions")]
    public int GotLogTransactions { get; set; }

    [JsonPropertyName("fetched")]
    public int Fetched { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("infobasePath")]
    public string InfobasePath { get; set; }
}
