using System;
using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationPeriodProduceJobRequest
{
    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; set; }

    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }

    [JsonPropertyName("objectFilters")]
    public string[] ObjectFilters { get; set; }

    [JsonPropertyName("transactionTypeFilters")]
    public string[] TransactionTypeFilters { get; set; }

    [JsonPropertyName("infobasePath")]
    public string InfobasePath { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("dataTypePropertyName")]
    public string DataTypePropertyName { get; set; }

#nullable enable
    [JsonPropertyName("ertRelativePath")]
    public string? ErtRelativePath { get; set; }

    [JsonPropertyName("documentGuidsDatabaseConnectionString")]
    public string? DocumentGuidsDatabaseConnectionString { get; set; }
}
