using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts;

public class RestartResult
{
    [JsonPropertyName("producers1C7Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C7Stopped { get; init; }

    [JsonPropertyName("producers1C7Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C7Started { get; init; }

    [JsonPropertyName("producers1C8Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C8Stopped { get; init; }

    [JsonPropertyName("producers1C8Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C8Started { get; init; }

    [JsonPropertyName("consumers1C7Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C7Stopped { get; init; }

    [JsonPropertyName("consumers1C7Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C7Started { get; init; }

    [JsonPropertyName("consumers1C8Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C8Stopped { get; init; }

    [JsonPropertyName("consumers1C8Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C8Started { get; init; }

    [JsonPropertyName("consumersMsSqlStopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ConsumersMsSqlStopped { get; init; }

    [JsonPropertyName("consumersMsSqlStarted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ConsumersMsSqlStarted { get; init; }

    [JsonPropertyName("consumerInstructionsCleared")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ConsumerInstructionsCleared { get; init; }
}
