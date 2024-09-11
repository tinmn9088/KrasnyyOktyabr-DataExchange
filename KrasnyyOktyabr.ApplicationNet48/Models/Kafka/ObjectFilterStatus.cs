#nullable enable

using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class ObjectFilterStatus(string name, int depth, string? topic)
{
    [JsonProperty("name")]
    public string Name { get; } = name;

    [JsonProperty("depth")]
    public int Depth { get; } = depth;

    [JsonProperty("topic")]
    public string? Topic { get; } = topic;
}
