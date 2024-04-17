using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class AbstractConsumerSettings
{
    [Required]
    public required string[] Topics { get; init; }

    public string? ConsumerGroup { get; init; }
}
