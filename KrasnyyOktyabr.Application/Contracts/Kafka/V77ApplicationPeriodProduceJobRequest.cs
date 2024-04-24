using System.Text.Json.Serialization;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class V77ApplicationPeriodProduceJobRequest
{
    [JsonPropertyName("start")]
    public required DateTimeOffset Start { get; init; }

    [JsonPropertyName("duration")]
    public required TimeSpan Duration { get; init; }

    [JsonPropertyName("objectFilters")]
    public required string[] ObjectFilters { get; init; }

    [JsonPropertyName("transactionTypeFilters")]
    public required string[] TransactionTypeFilters { get; init; }

    [JsonPropertyName("infobasePath")]
    public required string InfobasePath { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("password")]
    public required string Password { get; init; }

    [JsonPropertyName("dataTypePropertyName")]
    public required string DataTypePropertyName { get; init; }

    [JsonPropertyName("ertRelativePath")]
    public string? ErtRelativePath { get; init; }

    [JsonPropertyName("documentGuidsDatabaseConnectionString")]
    public string? DocumentGuidsDatabaseConnectionString { get; init; }
}
