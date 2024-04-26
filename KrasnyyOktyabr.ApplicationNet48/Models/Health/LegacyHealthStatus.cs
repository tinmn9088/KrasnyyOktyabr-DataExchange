#nullable enable

using System.Collections.Generic;
using System.Reflection;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Health;

public class LegacyHealthStatus
{
    private static readonly string? s_version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

    public LegacyHealthStatus()
    {
        Version = s_version;
    }

    [JsonProperty("version")]
    public string? Version { get; set; }

    [JsonProperty("producers")]
    public List<LegacyProducerHealthStatus>? Producers { get; set; }

    [JsonProperty("consumers")]
    public List<LegacyConsumerHealthStatus>? Consumers { get; set; }

    [JsonProperty("periodProduceJobs")]
    public List<V77ApplicationPeriodProduceJobStatus>? V77ApplicationPeriodProduceJobs { get; set; }

    [JsonProperty("connections1C7")]
    public List<ComV77ApplicationConnectionHealthStatus>? ComV77ApplicationConnections { get; set; }
}
