using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class HttpConsumerSettings : AbstractCredentialsConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:Http";

    [Required]
    public string Url { get; set; }
}
