using System.ComponentModel.DataAnnotations;
using MsSql;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class MsSqlConsumerSettings : AbstractConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:KrasnyyOktyabr.MsSql";

    [Required]
    public string ConnectionString { get; set; }

    [Required]
    public string TablePropertyName { get; set; }

#nullable enable
    public IMsSqlService.ConnectionType? ConnectionType { get; set; }
}
