using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class V77ApplicationProducerSettings : AbstractVApplicationProducerSettings
{
    public static string Position => "Kafka:Clients:Producers:1C7";

    [Required]
    public required string InfobasePath { get; init; }

    public string? ErtRelativePath { get; init; }
}
