using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class KafkaSettings
{
    public static string Position => "Kafka";

    [Required]
    public required string Socket { get; init; }

    public int? MessageMaxBytes { get; init; }

    public int? MaxPollIntervalMs { get; init; }
}
