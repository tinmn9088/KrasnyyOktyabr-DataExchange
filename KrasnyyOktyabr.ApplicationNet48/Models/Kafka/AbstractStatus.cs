using System;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class AbstractStatus
{

    /// <summary>
    /// Required for determining managed instance.
    /// </summary>
    public string ServiceKey { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("lastActivity")]
    public DateTimeOffset LastActivity { get; set; }

#nullable enable
    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }
}
