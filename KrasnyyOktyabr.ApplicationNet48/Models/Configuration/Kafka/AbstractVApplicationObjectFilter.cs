using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public abstract class AbstractVApplicationObjectFilter
{
    [Range(1, int.MaxValue, ErrorMessage = "Only natural numbers allowed")]
    public int JsonDepth { get; set; }

#nullable enable
    public string? Topic { get; set; }

    public string[]? TransactionTypeFilters { get; set; }
}
