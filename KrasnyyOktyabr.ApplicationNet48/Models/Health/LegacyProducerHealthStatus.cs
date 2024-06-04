#nullable enable

using System;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Health;

public class LegacyProducerHealthStatus
{
    [JsonProperty("__type")]
    public string? Type { get; set; }

    [JsonProperty("active")]
    public bool Active { get; set; }

    [JsonProperty("lastActivity")]
    public DateTimeOffset LastActivity { get; set; }

    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }

    [JsonProperty("objectFilters")]
    public string[]? ObjectFilters { get; set; }

    [JsonProperty("transactionTypes")]
    public string[]? TransactionTypes { get; set; }

    [JsonProperty("readFromLogFile")]
    public int? ReadFromLogFile { get; set; }

    [JsonProperty("fetched")]
    public int Fetched { get; set; }

    [JsonProperty("produced")]
    public int Produced { get; set; }

    [JsonProperty("infobase")]
    public string? InfobasePath { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("dataTypeJsonProperty")]
    public string? DataTypeJsonProperty { get; set; }

    [JsonProperty("suspendAt")]
    public TimePeriod[]? SuspendSchedule { get; set; }
}
