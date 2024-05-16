using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V83ApplicationConsumerStatus : AbstractConsumerStatus
{
    [JsonProperty("infobaseName")]
    public string InfobaseName { get; set; }

    [JsonProperty("saved")]
    public int Saved { get; set; }
}
