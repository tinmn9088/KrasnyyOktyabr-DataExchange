namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class V77ApplicationConsumerSettings : AbstractVApplicationConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:1C7";

    public required string InfobasePath { get; init; }
}
