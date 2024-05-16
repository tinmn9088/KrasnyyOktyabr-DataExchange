using System;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationPeriodProduceJobRequest
{
    [JsonProperty("start")]
    public DateTimeOffset Start { get; set; }

    [JsonProperty("duration")]
    public TimeSpan Duration { get; set; }

    [JsonProperty("objectFilters")]
    public string[] ObjectFilters { get; set; }

    [JsonProperty("transactionTypeFilters")]
    public string[] TransactionTypeFilters { get; set; }

    [JsonProperty("infobasePath")]
    public string InfobasePath { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("dataTypePropertyName")]
    public string DataTypePropertyName { get; set; }

#nullable enable
    [JsonProperty("ertRelativePath")]
    public string? ErtRelativePath { get; set; }

    [JsonProperty("documentGuidsDatabaseConnectionString")]
    public string? DocumentGuidsDatabaseConnectionString { get; set; }
}
