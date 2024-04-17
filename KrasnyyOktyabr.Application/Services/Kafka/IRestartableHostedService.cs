namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IRestartableHostedService<T> : IHostedService, IRestartable
{
    T Status { get; }
}
