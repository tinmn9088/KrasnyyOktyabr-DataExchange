using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static KrasnyyOktyabr.ApplicationNet48.Services.Kafka.V77ApplicationHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationPeriodProduceJobStatus
{
    [JsonProperty("lastActivity")]
    public DateTimeOffset LastActivity { get; set; }

    [JsonProperty("isCancellationRequested")]
    public bool IsCancellationRequested { get; set; }

#nullable enable
    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }
#nullable disable

    [JsonProperty("objectFilters")]
    public IReadOnlyList<ObjectFilter> ObjectFilters { get; set; }

    [JsonProperty("transactionTypeFilters")]
    public IReadOnlyList<string> TransactionTypeFilters { get; set; }

    [JsonProperty("produced")]
    public int Produced { get; set; }

    [JsonProperty("dataTypePropertyName")]
    public string DataTypePropertyName { get; set; }

    [JsonProperty("foundLogTransactions")]
    public int FoundLogTransactions { get; set; }

    [JsonProperty("fetched")]
    public int Fetched { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("infobasePath")]
    public string InfobasePath { get; set; }
}
