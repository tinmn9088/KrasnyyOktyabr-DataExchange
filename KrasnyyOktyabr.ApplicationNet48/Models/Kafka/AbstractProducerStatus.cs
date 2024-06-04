using System.Collections.Generic;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public abstract class AbstractProducerStatus : AbstractStatus
{
    [JsonProperty("transactionTypeFilters")]
    public IReadOnlyList<string> TransactionTypeFilters { get; set; }

    [JsonProperty("produced")]
    public int Produced { get; set; }

    [JsonProperty("dataTypePropertyName")]
    public string DataTypePropertyName { get; set; }

#nullable enable
    [JsonProperty("suspendAt")]
    public TimePeriod[]? SuspendSchedule { get; set; }
}
