using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class AbstractVApplicationProducerSettings
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string DataTypePropertyName { get; set; }

    /// <summary>
    /// Entries format - <c>"{id}:{depth}"</c>.
    /// </summary>
    [Required]
    public string[] ObjectFilters { get; set; }

    [Required]
    public string[] TransactionTypeFilters { get; set; }

#nullable enable
    public string? DocumentGuidsDatabaseConnectionString { get; set; }
}
