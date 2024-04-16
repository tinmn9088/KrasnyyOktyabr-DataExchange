namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class ConsumerMsSqlSettings : AbstractConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:MsSql";

    public required string ConnectionString { get; init; }

    public required string TableJsonPropertyName { get; init; }
}
