using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class V77ApplicationProducerSettings : AbstractVApplicationProducerSettings
{
    public static string Position => "Kafka:Clients:Producers:1C7";

    [Required]
    public string InfobasePath { get; set; }

    [Required]
    public V77ApplicationObjectFilter[] ObjectFilters { get; set; }

#nullable enable
    public string? ErtRelativePath { get; set; }

    public int? RetryTimes { get; set; }
}
