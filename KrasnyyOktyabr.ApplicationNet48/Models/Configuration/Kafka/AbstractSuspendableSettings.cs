#nullable enable

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public abstract class AbstractSuspendableSettings
{
    public TimePeriod[]? SuspendSchedule { get; set; }
}
