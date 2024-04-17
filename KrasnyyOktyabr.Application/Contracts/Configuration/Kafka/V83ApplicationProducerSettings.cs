using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class V83ApplicationProducerSettings : AbstractVApplicationProducerSettings
{
    public static string Position => "Kafka:Clients:Producers:1C8";

    [Required]
    public required string InfobaseUrl { get; init; }
}
