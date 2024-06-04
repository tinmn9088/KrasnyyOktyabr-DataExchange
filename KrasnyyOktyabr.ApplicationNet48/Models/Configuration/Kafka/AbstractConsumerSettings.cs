using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class AbstractConsumerSettings : AbstractSuspendableSettings
{
    [Required]
    public string[] Topics { get; set; }

    [Required]
    [ConfigurationKeyName("Instructions")]
    public Dictionary<string, string> TopicsInstructionNames { get; set; }

#nullable enable
    public string? ConsumerGroup { get; set; }
}
