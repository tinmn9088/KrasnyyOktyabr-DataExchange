#nullable enable

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class AbstractVApplicationConsumerSettings : AbstractConsumerSettings
{
    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? DocumentGuidsDatabaseConnectionString { get; set; }
}
