using System.Collections.Generic;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration;
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

#nullable enable
    [JsonProperty("suspendAt")]
    public TimePeriod[]? SuspendSchedule { get; set; }
}
