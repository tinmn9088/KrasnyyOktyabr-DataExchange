using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models;

public class RestartResult
{
    [JsonPropertyName("producers1C7Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C7Stopped { get; set; }

    [JsonPropertyName("producers1C7Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C7Started { get; set; }

    [JsonPropertyName("producers1C8Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C8Stopped { get; set; }

    [JsonPropertyName("producers1C8Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Producers1C8Started { get; set; }

    [JsonPropertyName("consumers1C7Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C7Stopped { get; set; }

    [JsonPropertyName("consumers1C7Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C7Started { get; set; }

    [JsonPropertyName("consumers1C8Stopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C8Stopped { get; set; }

    [JsonPropertyName("consumers1C8Started")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int Consumers1C8Started { get; set; }

    [JsonPropertyName("consumersMsSqlStopped")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ConsumersMsSqlStopped { get; set; }

    [JsonPropertyName("consumersMsSqlStarted")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ConsumersMsSqlStarted { get; set; }

    [JsonPropertyName("consumerInstructionsCleared")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public int ConsumerInstructionsCleared { get; set; }
}
