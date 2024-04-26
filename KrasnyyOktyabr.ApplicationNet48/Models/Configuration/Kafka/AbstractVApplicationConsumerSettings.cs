using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class AbstractVApplicationConsumerSettings : AbstractConsumerSettings
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

#nullable enable
    public string? DocumentGuidsDatabaseConnectionString { get; set; }
}
