using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class MsSqlConsumerSettings : AbstractConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:MsSql";

    [Required]
    public string ConnectionString { get; set; }

    [Required]
    public string TablePropertyName { get; set; }
}
