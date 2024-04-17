using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class AbstractVApplicationConsumerSettings : AbstractConsumerSettings
{
    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }

    public string? DocumentGuidsDatabaseConnectionString { get; init; }
}
