using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class V83ApplicationConsumerSettings : AbstractVApplicationConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:1C8";

    [Required]
    public required string InfobaseUrl { get; init; }
}
