using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class V77ApplicationConsumerSettings : AbstractVApplicationConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:1C7";

    [Required]
    public string InfobasePath { get; set; }

#nullable enable
    public string? ErtRelativePath { get; set; }
}
