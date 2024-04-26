using System;
using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class AbstractStatus
{

    /// <summary>
    /// Required for determining managed instance.
    /// </summary>
    [JsonIgnore]
    public string ServiceKey { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("lastActivity")]
    public DateTimeOffset LastActivity { get; set; }

#nullable enable
    [JsonPropertyName("errorMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }
}
