namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IRestartableHostedService<TStatus> : IHostedService, IRestartable
{
    TStatus Status { get; }
}
