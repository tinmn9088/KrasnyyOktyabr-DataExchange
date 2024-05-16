using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationConsumerStatus : AbstractConsumerStatus
{
    [JsonProperty("infobasePath")]
    public string InfobaseName { get; set; }

    [JsonProperty("saved")]
    public int Saved { get; set; }
}
