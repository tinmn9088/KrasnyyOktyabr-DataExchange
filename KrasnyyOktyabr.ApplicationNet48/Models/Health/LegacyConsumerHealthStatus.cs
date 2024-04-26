#nullable enable

using System;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Health;

public class LegacyConsumerHealthStatus
{
    [JsonProperty("__type")]
    public string? Type { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("lastActivity")]
    public DateTimeOffset LastActivity { get; set; }

    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonProperty("topics")]
    public string[]? Topics { get; set; }

    [JsonProperty("infobaseName")]
    public string? InfobaseName { get; set; }

    [JsonProperty("consumerGroup")]
    public string? ConsumerGroup { get; set; }

    [JsonProperty("consumed")]
    public int Consumed { get; set; }

    [JsonProperty("saved")]
    public int Saved { get; set; }

    [JsonProperty("tableJsonProperty")]
    public string? TableJsonProperty { get; set; }
}
