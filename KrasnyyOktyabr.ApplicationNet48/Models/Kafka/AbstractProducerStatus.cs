using System.Collections.Generic;
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
}
