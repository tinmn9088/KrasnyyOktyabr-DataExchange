namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class AbstractConsumerSettings
{
    public required string[] Topics { get; init; }
}
