#nullable enable

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class AbstractCredentialsConsumerSettings : AbstractConsumerSettings
{
    public string? Username { get; set; }

    public string? Password { get; set; }
}
