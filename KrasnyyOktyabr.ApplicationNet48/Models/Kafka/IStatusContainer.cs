using System.Collections.Generic;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Kafka;

public interface IStatusContainer<out TStatus> where TStatus : AbstractStatus
{
#nullable enable
    public IReadOnlyList<TStatus>? Statuses { get; }
}
