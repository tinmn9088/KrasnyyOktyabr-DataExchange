using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class MsSqlConsumerStatus : AbstractConsumerStatus
{
    [JsonProperty("saved")]
    public int Saved { get; set; }

    [JsonProperty("tablePropertyName")]
    public string TablePropertyName { get; set; }
}
