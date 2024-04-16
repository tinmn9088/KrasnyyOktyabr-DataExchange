namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IRestartableHostedService : IHostedService, IRestartable
{
}
