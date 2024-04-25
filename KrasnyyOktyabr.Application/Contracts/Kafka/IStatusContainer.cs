namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public interface IStatusContainer<out TStatus> where TStatus : AbstractStatus
{
    public IReadOnlyList<TStatus>? Statuses { get; }
}
