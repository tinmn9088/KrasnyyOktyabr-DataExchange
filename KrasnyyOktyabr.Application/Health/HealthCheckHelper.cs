using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using KrasnyyOktyabr.Application.Services.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using static KrasnyyOktyabr.Application.Services.Kafka.IV77ApplicationProducerService;

namespace KrasnyyOktyabr.Application.Health;

public static class HealthCheckHelper
{
    private static readonly string? s_version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

    /// <summary>
    /// Simulated old <i>WebService.REST</i> health check response.
    /// </summary>
    public static async Task WebServiceRESTResponseWriter(HttpContext context, HealthReport healthReport)
    {
        List<OldProducerHealthStatus>? producers = null;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            producers = GetV77ApplicationProducerStatuses(healthReport);
        }

        OldHealthStatus healthStatus = new()
        {
            Producers = producers,
        };

        await context.Response.WriteAsJsonAsync(healthStatus).ConfigureAwait(false);
    }

    [SupportedOSPlatform("windows")]
    private static List<OldProducerHealthStatus>? GetV77ApplicationProducerStatuses(HealthReport healthReport)
    {
        bool isV77ApplicationProducersPresent = healthReport.Entries.TryGetValue(
            key: nameof(V77ApplicationProducerService),
            out HealthReportEntry v77ApplicationProducerHealthStatus);

        if (!isV77ApplicationProducersPresent)
        {
            return null;
        }

        bool isDataPresent = v77ApplicationProducerHealthStatus.Data.TryGetValue(V77ApplicationProducerServiceHealthChecker.DataKey, out object? data);

        if (!isDataPresent)
        {
            return null;
        }

        if (data is not List<V77ApplicationProducerStatus> producerStatuses)
        {
            return null;
        }

        if (producerStatuses.Count == 0)
        {
            return null;
        }

        List<OldProducerHealthStatus> producers = [];

        foreach (V77ApplicationProducerStatus producerStatus in producerStatuses)
        {
            producers.Add(new OldProducerHealthStatus()
            {
                Type = nameof(V77ApplicationProducerService),
                Active = producerStatus.Active,
                LastActivity = producerStatus.LastActivity,
                ErrorMessage = producerStatus.ErrorMessage,
                ObjectIds = [.. producerStatus.ObjectFilters.Select(f => f.Id)],
                TransactionTypes = [.. producerStatus.TransactionTypes],
                ReadFromLogFile = producerStatus.GotLogTransactions,
                Fetched = producerStatus.Fetched,
                Produced = producerStatus.Produced,
                InfobasePath = producerStatus.InfobasePath,
                Username = producerStatus.Username,
                DataTypeJsonPropertyName = producerStatus.DataTypeJsonPropertyName,
            });
        }

        return producers;
    }

    private readonly struct OldHealthStatus
    {
        public OldHealthStatus()
        {
            Version = s_version;
        }

        [JsonPropertyName("version")]
        public string? Version { get; }

        [JsonPropertyName("producers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OldProducerHealthStatus>? Producers { get; init; }

        [JsonPropertyName("consumers")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OldConsumerHealthStatus>? Consumers { get; init; }

        [JsonPropertyName("connections1C7")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<OldComV77ApplicationConnectionHealthStatus>? ComV77ApplicationConnections { get; init; }
    }

    public readonly struct OldProducerHealthStatus
    {
        [JsonPropertyName("__type")]
        public required string Type { get; init; }

        [JsonPropertyName("active")]
        public required bool Active { get; init; }

        [JsonPropertyName("lastActivity")]
        public required DateTimeOffset LastActivity { get; init; }

        [JsonPropertyName("errorMessage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required string? ErrorMessage { get; init; }

        [JsonPropertyName("objectIds")]
        public required string[] ObjectIds { get; init; }

        [JsonPropertyName("transactionTypes")]
        public required string[] TransactionTypes { get; init; }

        [JsonPropertyName("readFromLogFile")]
        public required int ReadFromLogFile { get; init; }

        [JsonPropertyName("fetched")]
        public required int Fetched { get; init; }

        [JsonPropertyName("produced")]
        public required int Produced { get; init; }

        [JsonPropertyName("infobase")]
        public required string InfobasePath { get; init; }

        [JsonPropertyName("username")]
        public required string Username { get; init; }

        [JsonPropertyName("dataTypeJsonPropertyName")]
        public required string DataTypeJsonPropertyName { get; init; }
    }

    public readonly struct OldConsumerHealthStatus
    {

    }

    public readonly struct OldComV77ApplicationConnectionHealthStatus
    {

    }
}
