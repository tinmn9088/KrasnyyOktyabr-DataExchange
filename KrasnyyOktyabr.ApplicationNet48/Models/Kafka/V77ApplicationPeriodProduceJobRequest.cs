using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class V77ApplicationPeriodProduceJobRequest
{
    [Required]
    [JsonProperty("start")]
    public DateTimeOffset Start { get; set; }

    [Required]
    [JsonProperty("duration")]
    public TimeSpan Duration { get; set; }

    [Required]
    [JsonProperty("objectFilters")]
    public string[] ObjectFilters { get; set; }

    [Required]
    [JsonProperty("transactionTypeFilters")]
    public string[] TransactionTypeFilters { get; set; }

    [Required]
    [JsonProperty("infobasePath")]
    public string InfobasePath { get; set; }

    [Required]
    [JsonProperty("username")]
    public string Username { get; set; }

    [Required]
    [JsonProperty("password")]
    public string Password { get; set; }

    [Required]
    [JsonProperty("dataTypePropertyName")]
    public string DataTypePropertyName { get; set; }

#nullable enable
    [JsonProperty("ertRelativePath")]
    public string? ErtRelativePath { get; set; }

    [JsonProperty("documentGuidsDatabaseConnectionString")]
    public string? DocumentGuidsDatabaseConnectionString { get; set; }
}
