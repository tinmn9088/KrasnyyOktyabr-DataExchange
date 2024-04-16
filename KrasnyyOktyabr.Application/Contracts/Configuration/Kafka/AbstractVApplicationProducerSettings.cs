namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class AbstractVApplicationProducerSettings
{
    public required string Username { get; init; }

    public required string Password { get; init; }

    public required string DataTypeJsonPropertyName { get; init; }

    /// <summary>
    /// Entries format - <c>"{id}:{depth}"</c>.
    /// </summary>
    public required string[] ObjectFilters { get; init; }

    public required string[] TransactionTypeFilters { get; init; }

    public string? DocumentGuidsDatabaseConnectionString { get; init; }
}
