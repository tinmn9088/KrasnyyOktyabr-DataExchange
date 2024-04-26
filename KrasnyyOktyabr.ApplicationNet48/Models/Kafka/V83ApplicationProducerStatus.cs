using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V83ApplicationProducerStatus : AbstractProducerStatus
{
    [JsonPropertyName("objectFilters")]
    public IReadOnlyList<string> ObjectFilters { get; set; }

    [JsonPropertyName("fetched")]
    public int Fetched { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("infobaseUrl")]
    public string InfobaseUrl { get; set; }
}
