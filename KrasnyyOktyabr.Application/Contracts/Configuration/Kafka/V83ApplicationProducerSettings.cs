namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class V83ApplicationProducerSettings : AbstractVApplicationProducerSettings
{
    public static string Position => "Kafka:Clients:Producers:1C8";

    public required string InfobaseUrl { get; init; }
}
