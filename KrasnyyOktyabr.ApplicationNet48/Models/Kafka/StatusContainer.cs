using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public class StatusContainer<TStatus> : IStatusContainer<TStatus> where TStatus : AbstractStatus
{
    public static readonly StatusContainer<TStatus> Empty = new() { Statuses = null, };

#nullable enable
    [JsonPropertyName("statuses")]
    public IReadOnlyList<TStatus>? Statuses { get; set; }
}
