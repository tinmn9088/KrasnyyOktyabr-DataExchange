using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class V77ApplicationObjectFilter : AbstractVApplicationObjectFilter
{
    [Required]
    public string IdPrefix { get; set; }

    public bool ReadLastOnly { get; set; }
}
