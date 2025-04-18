using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class HttpConsumerStatus : AbstractConsumerStatus
{
    [JsonProperty("url")]
    public string Url { get; set; }

    [JsonProperty("sent")]
    public int Sent { get; set; }
}
