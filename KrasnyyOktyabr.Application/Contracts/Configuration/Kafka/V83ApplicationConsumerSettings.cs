namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class V83ApplicationConsumerSettings : AbstractVApplicationConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:1C8";

    public required string InfobaseUrl { get; init; }
}
