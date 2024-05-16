using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V83ApplicationProducerStatus : AbstractProducerStatus
{
    [JsonProperty("objectFilters")]
    public IReadOnlyList<string> ObjectFilters { get; set; }

    [JsonProperty("fetched")]
    public int Fetched { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("infobaseUrl")]
    public string InfobaseUrl { get; set; }
}
