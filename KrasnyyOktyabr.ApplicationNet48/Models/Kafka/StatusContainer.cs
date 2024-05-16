using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class StatusContainer<TStatus> : IStatusContainer<TStatus> where TStatus : AbstractStatus
{
    public static readonly StatusContainer<TStatus> Empty = new() { Statuses = null, };

#nullable enable
    [JsonProperty("statuses")]
    public IReadOnlyList<TStatus>? Statuses { get; set; }
}
