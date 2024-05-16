using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models;

public class RestartResult
{
    [JsonProperty("producers1C7Stopped")]
    public int Producers1C7Stopped { get; set; }

    [JsonProperty("producers1C7Started")]
    public int Producers1C7Started { get; set; }

    [JsonProperty("producers1C8Stopped")]
    public int Producers1C8Stopped { get; set; }

    [JsonProperty("producers1C8Started")]
    public int Producers1C8Started { get; set; }

    [JsonProperty("consumers1C7Stopped")]
    public int Consumers1C7Stopped { get; set; }

    [JsonProperty("consumers1C7Started")]
    public int Consumers1C7Started { get; set; }

    [JsonProperty("consumers1C8Stopped")]
    public int Consumers1C8Stopped { get; set; }

    [JsonProperty("consumers1C8Started")]
    public int Consumers1C8Started { get; set; }

    [JsonProperty("consumersMsSqlStopped")]
    public int ConsumersMsSqlStopped { get; set; }

    [JsonProperty("consumersMsSqlStarted")]
    public int ConsumersMsSqlStarted { get; set; }

    [JsonProperty("consumerInstructionsCleared")]
    public int ConsumerInstructionsCleared { get; set; }
}
