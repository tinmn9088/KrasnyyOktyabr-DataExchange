using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class StatusContainer<TStatus> : IStatusContainer<TStatus> where TStatus : AbstractStatus
{
    public static readonly StatusContainer<TStatus> Empty = new() { Statuses = null, };

    [JsonPropertyName("statuses")]
    public required IReadOnlyList<TStatus>? Statuses { get; init; }
}
