using System.ComponentModel.DataAnnotations;
using static KrasnyyOktyabr.ApplicationNet48.Services.IMsSqlService;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class MsSqlConsumerSettings : AbstractConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:MsSql";

    [Required]
    public string ConnectionString { get; set; }

    [Required]
    public string TablePropertyName { get; set; }

#nullable enable
    public ConnectionType? ConnectionType { get; set; }
}
