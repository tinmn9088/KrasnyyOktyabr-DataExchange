using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class AbstractVApplicationProducerSettings
{
    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }

    [Required]
    public required string DataTypePropertyName { get; init; }

    /// <summary>
    /// Entries format - <c>"{id}:{depth}"</c>.
    /// </summary>
    [Required]
    public required string[] ObjectFilters { get; init; }

    [Required]
    public required string[] TransactionTypeFilters { get; init; }

    public string? DocumentGuidsDatabaseConnectionString { get; init; }
}
