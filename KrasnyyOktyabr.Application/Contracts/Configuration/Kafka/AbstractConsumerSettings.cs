using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class AbstractConsumerSettings
{
    [Required]
    public required string[] Topics { get; init; }

    [Required]
    public required string DataTypePropertyName { get; init; }

    [Required]
    [ConfigurationKeyName("Instructions")]
    public required Dictionary<string, string> TopicsInstructionNames { get; init; }

    public string? ConsumerGroup { get; init; }
}
