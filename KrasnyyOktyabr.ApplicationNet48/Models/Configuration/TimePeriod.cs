using System;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration;

public class TimePeriod
{
    [JsonProperty("start")]
    public TimeSpan Start { get; set; }

    [JsonProperty("duration")]
    public TimeSpan Duration { get; set; }
}
