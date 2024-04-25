using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text.Json.Serialization;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using KrasnyyOktyabr.Application.Services.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using static KrasnyyOktyabr.ComV77Application.IComV77ApplicationConnectionFactory;

namespace KrasnyyOktyabr.Application.Health;

public static class HealthCheckHelper
{
    private static readonly string? s_version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

    /// <summary>
    /// Simulated old <i>WebService.REST</i> health check response.
    /// </summary>
    public static async Task WebServiceRESTResponseWriter(HttpContext context, HealthReport healthReport)
    {
        List<OldProducerHealthStatus>? producerStatuses = [];
        List<OldConsumerHealthStatus>? consumerStatuses = [];
        List<OldComV77ApplicationConnectionHealthStatus>? comV77ApplicationConnectionStatuses = null;
        List<V77ApplicationPeriodProduceJobStatus>? v77ApplicationPeriodProduceJobStatuses = null;

        AddProducerStatuses(GetV83ApplicationProducerStatuses, healthReport, ref producerStatuses);

        AddConsumerStatuses(GetV83ApplicationConsumerStatuses, healthReport, ref consumerStatuses);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            AddProducerStatuses(GetV77ApplicationProducerStatuses, healthReport, ref producerStatuses);

            AddConsumerStatuses(GetMsSqlConsumerStatuses, healthReport, ref consumerStatuses);

            AddConsumerStatuses(GetV77ApplicationConsumerStatuses, healthReport, ref consumerStatuses);

            comV77ApplicationConnectionStatuses = GetComV77ApplicationConnectionHealthStatuses(healthReport);

            v77ApplicationPeriodProduceJobStatuses = GetV77ApplicationPeriodProduceJobStatuses(healthReport);
        }

        OldHealthStatus healthStatus = new()
        {
            Producers = producerStatuses.Count > 0 ? producerStatuses : null,
            Consumers = consumerStatuses.Count > 0 ? consumerStatuses : null,
            ComV77ApplicationConnections = comV77ApplicationConnectionStatuses,
            V77ApplicationPeriodProduceJobs = v77ApplicationPeriodProduceJobStatuses,
        };

        await context.Response.WriteAsJsonAsync(healthStatus).ConfigureAwait(false);
    }

    private static void AddProducerStatuses(ProducerStatusesGatherer gatherer, HealthReport healthReport, ref List<OldProducerHealthStatus> statuses)
    {
        List<OldProducerHealthStatus>? gatheredStatuses = gatherer(healthReport);

        if (gatheredStatuses != null)
        {
            statuses.AddRange(gatheredStatuses);
        }
    }

    private static void AddConsumerStatuses(ConsumerStatusesGatherer gatherer, HealthReport healthReport, ref List<OldConsumerHealthStatus> statuses)
    {
        List<OldConsumerHealthStatus>? gatheredStatuses = gatherer(healthReport);

        if (gatheredStatuses != null)
        {
            statuses.AddRange(gatheredStatuses);
        }
    }

    [SupportedOSPlatform("windows")]
    private static List<OldProducerHealthStatus>? GetV77ApplicationProducerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V77ApplicationProducerStatus>? statuses = GetStatusFromHealthReport<V77ApplicationProducerStatus>(
            healthReport,
            dataKey: V77ApplicationProducerServiceHealthChecker.DataKey);

        if (statuses == null)
        {
            return null;
        }

        List<OldProducerHealthStatus> oldStatuses = [];

        foreach (V77ApplicationProducerStatus status in statuses)
        {
            oldStatuses.Add(new OldProducerHealthStatus()
            {
                Type = nameof(V77ApplicationProducerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                ObjectIds = [.. status.ObjectFilters.Select(f => f.Id)],
                TransactionTypes = [.. status.TransactionTypeFilters],
                ReadFromLogFile = status.GotLogTransactions,
                Fetched = status.Fetched,
                Produced = status.Produced,
                InfobasePath = status.InfobasePath,
                Username = status.Username,
                DataTypeJsonPropertyName = status.DataTypePropertyName,
            });
        }

        return oldStatuses;
    }

    private static List<OldProducerHealthStatus>? GetV83ApplicationProducerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V83ApplicationProducerStatus>? statuses = GetStatusFromHealthReport<V83ApplicationProducerStatus>(
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
                TransactionTypes = [.. status.TransactionTypeFilters],
                Fetched = status.Fetched,
                Produced = status.Produced,
                InfobasePath = status.InfobaseUrl,
                Username = status.Username,
                DataTypeJsonPropertyName = status.DataTypePropertyName,

                // Unused
                ReadFromLogFile = null,
            });
        }

        return oldStatuses;
    }

    [SupportedOSPlatform("windows")]
    private static List<OldConsumerHealthStatus>? GetMsSqlConsumerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<MsSqlConsumerStatus>? statuses = GetStatusFromHealthReport<MsSqlConsumerStatus>(
            healthReport,
            dataKey: MsSqlConsumerServiceHealthChecker.DataKey);

        if (statuses == null)
        {
            return null;
        }

        List<OldConsumerHealthStatus> oldStatuses = [];

        foreach (MsSqlConsumerStatus status in statuses)
        {
            oldStatuses.Add(new OldConsumerHealthStatus()
            {
                Type = nameof(MsSqlConsumerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                Consumed = status.Consumed,
                Saved = status.Saved,
                TableJsonPropertyName = status.TablePropertyName,
                Topics = [.. status.Topics],
                ConsumerGroup = status.ConsumerGroup,
            });
        }

        return oldStatuses;
    }

    [SupportedOSPlatform("windows")]
    private static List<OldConsumerHealthStatus>? GetV77ApplicationConsumerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V77ApplicationConsumerStatus>? statuses = GetStatusFromHealthReport<V77ApplicationConsumerStatus>(
            healthReport,
            dataKey: V77ApplicationConsumerServiceHealthChecker.DataKey);

        if (statuses == null)
        {
            return null;
        }

        List<OldConsumerHealthStatus> oldStatuses = [];

        foreach (V77ApplicationConsumerStatus status in statuses)
        {
            oldStatuses.Add(new OldConsumerHealthStatus()
            {
                Type = nameof(V77ApplicationConsumerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                Consumed = status.Consumed,
                Saved = status.Saved,
                InfobaseName = status.InfobaseName,
                Topics = [.. status.Topics],
                ConsumerGroup = status.ConsumerGroup,
            });
        }

        return oldStatuses;
    }

    private static List<OldConsumerHealthStatus>? GetV83ApplicationConsumerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V83ApplicationConsumerStatus>? statuses = GetStatusFromHealthReport<V83ApplicationConsumerStatus>(
            healthReport,
            dataKey: V83ApplicationConsumerServiceHealthChecker.DataKey);

        if (statuses == null)
        {
            return null;
        }

        List<OldConsumerHealthStatus> oldStatuses = [];

        foreach (V83ApplicationConsumerStatus status in statuses)
        {
            oldStatuses.Add(new OldConsumerHealthStatus()
            {
                Type = nameof(V83ApplicationConsumerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                Consumed = status.Consumed,
                Saved = status.Saved,
                InfobaseName = status.InfobaseName,
                Topics = [.. status.Topics],
                ConsumerGroup = status.ConsumerGroup,
            });
        }

        return oldStatuses;
    }

    private static IReadOnlyList<TStatus>? GetStatusFromHealthReport<TStatus>(HealthReport healthReport, string dataKey) where TStatus : AbstractStatus
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

        if (data is not IStatusContainer<TStatus> serviceStatus)
        {
            return null;
        }

        if (serviceStatus.Statuses == null || serviceStatus.Statuses.Count == 0)
        {
            return null;
        }

        return serviceStatus.Statuses;
    }

    [SupportedOSPlatform("windows")]
    private static List<OldComV77ApplicationConnectionHealthStatus>? GetComV77ApplicationConnectionHealthStatuses(HealthReport healthReport)
    {
        bool isStatusPresent = healthReport.Entries.TryGetValue(
            key: nameof(ComV77ApplicationConnectionFactoryStatus),
            out HealthReportEntry status);

        if (!isStatusPresent)
        {
            return null;
        }

        bool isDataPresent = status.Data.TryGetValue(ComV77ApplicationConnectionFactoryHealthChecker.DataKey, out object? data);

        if (!isDataPresent)
        {
            return null;
        }

        if (data is not ComV77ApplicationConnectionFactoryStatus factoryStatus)
        {
            return null;
        }

        if (factoryStatus.Connections.Length == 0)
        {
            return null;
        }

        return factoryStatus.Connections
            .Select(s => new OldComV77ApplicationConnectionHealthStatus()
            {
                InfobasePath = s.InfobasePath,
                Username = s.Username,
                ErrorsCount = s.ErrorsCount,
                RetievedTimes = s.RetievedTimes,
                IsInitialized = s.IsInitialized,
                IsDisposed = s.IsDisposed,
                LastTimeDisposed = s.LastTimeDisposed,
            })
            .ToList();
    }

    [SupportedOSPlatform("windows")]
    private static List<V77ApplicationPeriodProduceJobStatus>? GetV77ApplicationPeriodProduceJobStatuses(HealthReport healthReport)
    {
        bool isStatusPresent = healthReport.Entries.TryGetValue(
            key: nameof(V77ApplicationPeriodProduceJobStatus),
            out HealthReportEntry status);

        if (!isStatusPresent)
        {
            return null;
        }

        bool isDataPresent = status.Data.TryGetValue(V77ApplicationPeriodProduceJobServiceHealthChecker.DataKey, out object? data);

        if (!isDataPresent)
        {
            return null;
        }

        if (data is not List<V77ApplicationPeriodProduceJobStatus> jobsStatuses)
        {
            return null;
        }

        return jobsStatuses.Count == 0 ? null : jobsStatuses;
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

        [JsonPropertyName("periodProduceJobs")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<V77ApplicationPeriodProduceJobStatus>? V77ApplicationPeriodProduceJobs { get; init; }

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
        [JsonPropertyName("__type")]
        public required string Type { get; init; }

        [JsonPropertyName("active")]
        public required bool Active { get; init; }

        [JsonPropertyName("lastActivity")]
        public required DateTimeOffset LastActivity { get; init; }

        [JsonPropertyName("errorMessage")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required string? ErrorMessage { get; init; }

        [JsonPropertyName("topics")]
        public required string[] Topics { get; init; }

        [JsonPropertyName("infobaseName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? InfobaseName { get; init; }

        [JsonPropertyName("consumerGroup")]
        public required string ConsumerGroup { get; init; }

        [JsonPropertyName("consumed")]
        public required int Consumed { get; init; }

        [JsonPropertyName("saved")]
        public required int Saved { get; init; }

        [JsonPropertyName("tableJsonPropertyName")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? TableJsonPropertyName { get; init; }
    }

    public readonly struct OldComV77ApplicationConnectionHealthStatus
    {
        [JsonPropertyName("path")]
        public required string InfobasePath { get; init; }

        [JsonPropertyName("username")]
        public required string Username { get; init; }

        [JsonPropertyName("lastTimeDisposed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public required DateTimeOffset? LastTimeDisposed { get; init; }

        [JsonPropertyName("initialized")]
        public required bool IsInitialized { get; init; }

        [JsonPropertyName("disposed")]
        public required bool IsDisposed { get; init; }

        [JsonPropertyName("retrieved")]
        public required int RetievedTimes { get; init; }

        [JsonPropertyName("errorsCount")]
        public required int ErrorsCount { get; init; }
    }

    private delegate List<OldProducerHealthStatus>? ProducerStatusesGatherer(HealthReport healthReport);

    private delegate List<OldConsumerHealthStatus>? ConsumerStatusesGatherer(HealthReport healthReport);
}
