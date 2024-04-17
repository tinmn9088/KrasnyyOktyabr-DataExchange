using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using KrasnyyOktyabr.Application.Services.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using static KrasnyyOktyabr.Application.Services.Kafka.IV77ApplicationProducerService;
using static KrasnyyOktyabr.Application.Services.Kafka.IV83ApplicationProducerService;

namespace KrasnyyOktyabr.Application.Health;

public static class HealthCheckHelper
{
    private static readonly string? s_version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

    /// <summary>
    /// Simulated old <i>WebService.REST</i> health check response.
    /// </summary>
    public static async Task WebServiceRESTResponseWriter(HttpContext context, HealthReport healthReport)
    {
        List<OldProducerHealthStatus>? statuses = [];

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            AddStatuses(GetV77ApplicationProducerStatuses, healthReport, ref statuses);
        }

        AddStatuses(GetV83ApplicationProducerStatuses, healthReport, ref statuses);

        OldHealthStatus healthStatus = new()
        {
            Producers = statuses,
        };

        await context.Response.WriteAsJsonAsync(healthStatus).ConfigureAwait(false);
    }

    private static void AddStatuses(StatusGatherer gatherer, HealthReport healthReport, ref List<OldProducerHealthStatus> statuses)
    {
        List<OldProducerHealthStatus>? gatheredStatuses = gatherer(healthReport);

        if (gatheredStatuses != null)
        {
            statuses.AddRange(gatheredStatuses);
        }
    }

    [SupportedOSPlatform("windows")]
    private static List<OldProducerHealthStatus>? GetV77ApplicationProducerStatuses(HealthReport healthReport)
    {
        List<V77ApplicationProducerStatus>? statuses = GetStatusFromHealthReport<V77ApplicationProducerStatus>(
            healthReport,
            dataKey: V77ApplicationProducerServiceHealthChecker.DataKey);

        if (statuses == null)
        {
            return null;
        }

        List<OldProducerHealthStatus> producers = [];

        foreach (V77ApplicationProducerStatus status in statuses)
        {
            producers.Add(new OldProducerHealthStatus()
            {
                Type = nameof(V77ApplicationProducerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                ObjectIds = [.. status.ObjectFilters.Select(f => f.Id)],
                TransactionTypes = [.. status.TransactionTypes],
                ReadFromLogFile = status.GotLogTransactions,
                Fetched = status.Fetched,
                Produced = status.Produced,
                InfobasePath = status.InfobasePath,
                Username = status.Username,
                DataTypeJsonPropertyName = status.DataTypeJsonPropertyName,
            });
        }

        return producers;
    }

    private static List<OldProducerHealthStatus>? GetV83ApplicationProducerStatuses(HealthReport healthReport)
    {
        List<V83ApplicationProducerStatus>? statuses = GetStatusFromHealthReport<V83ApplicationProducerStatus>(
            healthReport,
            dataKey: V83ApplicationProducerServiceHealthChecker.DataKey);

        if (statuses == null)
        {
            return null;
        }

        List<OldProducerHealthStatus> oldStatuses = [];

        foreach (V83ApplicationProducerStatus status in statuses)
        {
            oldStatuses.Add(new OldProducerHealthStatus()
            {
                Type = nameof(V83ApplicationProducerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                ObjectIds = [.. status.ObjectFilters],
                TransactionTypes = [.. status.TransactionTypes],
                Fetched = status.Fetched,
                Produced = status.Produced,
                InfobasePath = status.InfobasePath,
                Username = status.Username,
                DataTypeJsonPropertyName = status.DataTypeJsonPropertyName,

                // Unused
                ReadFromLogFile = null,
            });
        }

        return oldStatuses;
    }

    private static List<TStatus>? GetStatusFromHealthReport<TStatus>(HealthReport healthReport, string dataKey)
    {
        bool isStatusPresent = healthReport.Entries.TryGetValue(
            key: typeof(TStatus).Name,
            out HealthReportEntry status);

        if (!isStatusPresent)
        {
            return null;
        }

        bool isDataPresent = status.Data.TryGetValue(dataKey, out object? data);

        if (!isDataPresent)
        {
            return null;
        }

        if (data is not List<TStatus> statuses)
        {
            return null;
        }

        if (statuses.Count == 0)
        {
            return null;
        }

        return statuses;
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
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required int? ReadFromLogFile { get; init; }

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

    private delegate List<OldProducerHealthStatus>? StatusGatherer(HealthReport healthReport);
}
