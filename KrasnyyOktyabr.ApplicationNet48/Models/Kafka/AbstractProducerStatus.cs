using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public abstract class AbstractProducerStatus : AbstractStatus
{
    [JsonPropertyName("transactionTypeFilters")]
    public IReadOnlyList<string> TransactionTypeFilters { get; set; }

    [JsonPropertyName("produced")]
    public int Produced { get; set; }

    [JsonPropertyName("dataTypePropertyName")]
    public string DataTypePropertyName { get; set; }
}
