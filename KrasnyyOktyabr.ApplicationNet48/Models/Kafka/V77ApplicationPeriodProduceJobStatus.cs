using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static KrasnyyOktyabr.ApplicationNet48.Services.Kafka.V77ApplicationProducersHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationPeriodProduceJobStatus
{
    [JsonPropertyName("lastActivity")]
    public DateTimeOffset LastActivity { get; set; }

    [JsonPropertyName("isCancellationRequested")]
    public bool IsCancellationRequested { get; set; }

#nullable enable
    [JsonPropertyName("errorMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }
#nullable disable

    [JsonPropertyName("objectFilters")]
    public IReadOnlyList<ObjectFilter> ObjectFilters { get; set; }

    [JsonPropertyName("transactionTypeFilters")]
    public IReadOnlyList<string> TransactionTypeFilters { get; set; }

    [JsonPropertyName("produced")]
    public int Produced { get; set; }

    [JsonPropertyName("dataTypePropertyName")]
    public string DataTypePropertyName { get; set; }

    [JsonPropertyName("foundLogTransactions")]
    public int FoundLogTransactions { get; set; }

    [JsonPropertyName("fetched")]
    public int Fetched { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("infobasePath")]
    public string InfobasePath { get; set; }
}
