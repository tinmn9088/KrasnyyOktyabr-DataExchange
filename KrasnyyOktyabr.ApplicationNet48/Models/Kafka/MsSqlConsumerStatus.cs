using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class MsSqlConsumerStatus : AbstractConsumerStatus
{
    [JsonPropertyName("saved")]
    public int Saved { get; set; }

    [JsonPropertyName("tablePropertyName")]
    public string TablePropertyName { get; set; }
}
