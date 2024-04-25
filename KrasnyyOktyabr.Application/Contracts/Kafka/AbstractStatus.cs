using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class AbstractStatus
{
    /// <summary>
    /// Required for determining managed instance.
    /// </summary>
    [JsonIgnore]
    public required string ServiceKey { get; init; }

    [JsonPropertyName("active")]
    public required bool Active { get; init; }

    [JsonPropertyName("lastActivity")]
    public required DateTimeOffset LastActivity { get; init; }

    [JsonPropertyName("errorMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? ErrorMessage { get; init; }
}
