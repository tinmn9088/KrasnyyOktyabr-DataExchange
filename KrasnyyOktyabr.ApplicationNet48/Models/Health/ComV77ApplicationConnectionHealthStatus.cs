using System;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Health;

public class ComV77ApplicationConnectionHealthStatus
{
    [JsonProperty("path")]
    public string InfobasePath { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("lastTimeDisposed")]
    public DateTimeOffset? LastTimeDisposed { get; set; }

    [JsonProperty("isInitialized")]
    public bool IsInitialized { get; set; }

    [JsonProperty("disposed")]
    public bool IsDisposed { get; set; }

    [JsonProperty("retrieved")]
    public int RetrievedTimes { get; set; }

    [JsonProperty("errorsCount")]
    public int ErrorsCount { get; set; }
}
