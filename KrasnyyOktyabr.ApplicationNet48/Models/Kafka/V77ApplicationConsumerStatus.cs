using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationConsumerStatus : AbstractConsumerStatus
{
    [JsonPropertyName("infobasePath")]
    public string InfobaseName { get; set; }

    [JsonPropertyName("saved")]
    public int Saved { get; set; }
}
