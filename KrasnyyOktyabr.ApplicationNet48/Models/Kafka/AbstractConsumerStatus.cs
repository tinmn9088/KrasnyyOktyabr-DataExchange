using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class AbstractConsumerStatus : AbstractStatus
{
    [JsonPropertyName("consumed")]
    public int Consumed { get; set; }

    [JsonPropertyName("topics")]
    public IReadOnlyList<string> Topics { get; set; }

    [JsonPropertyName("consumerGroup")]
    public string ConsumerGroup { get; set; }
}
