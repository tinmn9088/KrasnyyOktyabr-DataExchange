using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationObjectFilterStatus(string name, int depth, string topic, bool readLastOnly) : ObjectFilterStatus(name, depth, topic)
{
    [JsonProperty("readLastOnly", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool ReadLastOnly { get; } = readLastOnly;
}
