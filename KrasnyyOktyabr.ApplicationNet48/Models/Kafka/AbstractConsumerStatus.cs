using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class AbstractConsumerStatus : AbstractStatus
{
    [JsonProperty("consumed")]
    public int Consumed { get; set; }

    [JsonProperty("topics")]
    public IReadOnlyList<string> Topics { get; set; }

    [JsonProperty("consumerGroup")]
    public string ConsumerGroup { get; set; }
}
