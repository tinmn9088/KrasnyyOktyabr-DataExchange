using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public readonly struct ObjectFilter(string id, int depth)
{
    [JsonProperty("id")]
    public string Id { get; } = id;

    [JsonProperty("depth")]
    public int Depth { get; } = depth;
}
