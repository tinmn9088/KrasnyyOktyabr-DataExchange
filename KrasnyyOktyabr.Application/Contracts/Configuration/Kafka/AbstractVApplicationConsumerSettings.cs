namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class AbstractVApplicationConsumerSettings : AbstractConsumerSettings
{
    public required string Username { get; init; }

    public required string Password { get; init; }

    public string? DocumentGuidsDatabaseConnectionString { get; init; }
}
