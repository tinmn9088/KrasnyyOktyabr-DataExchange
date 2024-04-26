using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class KafkaSettings
{
    public static string Position => "Kafka";

    [Required]
    public string Socket { get; set; }

#nullable enable
    public int? MessageMaxBytes { get; set; }

    public int? MaxPollIntervalMs { get; set; }
}
