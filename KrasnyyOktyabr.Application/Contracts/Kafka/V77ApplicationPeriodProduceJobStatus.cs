using System.Text.Json.Serialization;
using static KrasnyyOktyabr.Application.Services.Kafka.V77ApplicationProducersHelper;

namespace KrasnyyOktyabr.Application.Contracts.Kafka;

public class V77ApplicationPeriodProduceJobStatus
{
    [JsonPropertyName("lastActivity")]
    public required DateTimeOffset LastActivity { get; init; }

    [JsonPropertyName("isCancellationRequested")]
    public required bool IsCancellationRequested { get; init; }

    [JsonPropertyName("errorMessage")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public required string? ErrorMessage { get; init; }

    [JsonPropertyName("objectFilters")]
    public required IReadOnlyList<ObjectFilter> ObjectFilters { get; init; }

    [JsonPropertyName("transactionTypeFilters")]
    public required IReadOnlyList<string> TransactionTypeFilters { get; init; }

    [JsonPropertyName("produced")]
    public required int Produced { get; init; }

    [JsonPropertyName("dataTypePropertyName")]
    public required string DataTypePropertyName { get; init; }

    [JsonPropertyName("foundLogTransactions")]
    public required int FoundLogTransactions { get; init; }

    [JsonPropertyName("fetched")]
    public required int Fetched { get; init; }

    [JsonPropertyName("username")]
    public required string Username { get; init; }

    [JsonPropertyName("infobasePath")]
    public required string InfobasePath { get; init; }
}
