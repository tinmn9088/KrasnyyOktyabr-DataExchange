using System.Collections.Generic;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationProducerStatus : AbstractProducerStatus
{
    [JsonProperty("objectFilters")]
    public IReadOnlyList<ObjectFilter> ObjectFilters { get; set; }

    [JsonProperty("gotLogTransactions")]
    public int GotLogTransactions { get; set; }

    [JsonProperty("fetched")]
    public int Fetched { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("infobasePath")]
    public string InfobasePath { get; set; }
}
