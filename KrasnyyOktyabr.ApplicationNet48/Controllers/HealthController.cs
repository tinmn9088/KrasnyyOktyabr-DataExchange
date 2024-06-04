#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using KrasnyyOktyabr.ApplicationNet48.Health;
using KrasnyyOktyabr.ApplicationNet48.Models.Health;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using static KrasnyyOktyabr.ComV77Application.IComV77ApplicationConnectionFactory;
using static Confluent.Kafka.ConfigPropertyNames;
namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

public class HealthController(HealthCheckService healthCheckService) : ApiController
{
    public async Task<IHttpActionResult> GetHealthStatus(CancellationToken cancellationToken)
    {
        HealthReport healthReport = await healthCheckService.CheckHealthAsync(cancellationToken);

        List<LegacyProducerHealthStatus>? producerStatuses = [];
        List<LegacyConsumerHealthStatus>? consumerStatuses = [];
        List<ComV77ApplicationConnectionHealthStatus>? comV77ApplicationConnectionStatuses = null;
        List<V77ApplicationPeriodProduceJobStatus>? v77ApplicationPeriodProduceJobStatuses = null;

        AddConsumerStatuses(GetMsSqlConsumerStatuses, healthReport, ref consumerStatuses);

        AddProducerStatuses(GetV83ApplicationProducerStatuses, healthReport, ref producerStatuses);

        AddConsumerStatuses(GetV83ApplicationConsumerStatuses, healthReport, ref consumerStatuses);

        AddProducerStatuses(GetV77ApplicationProducerStatuses, healthReport, ref producerStatuses);

        AddConsumerStatuses(GetV77ApplicationConsumerStatuses, healthReport, ref consumerStatuses);

        v77ApplicationPeriodProduceJobStatuses = GetV77ApplicationPeriodProduceJobStatuses(healthReport);

        comV77ApplicationConnectionStatuses = GetComV77ApplicationConnectionHealthStatuses(healthReport);

        return Json(new LegacyHealthStatus()
        {
            Producers = producerStatuses.Count > 0 ? producerStatuses : null,
            Consumers = consumerStatuses.Count > 0 ? consumerStatuses : null,
            ComV77ApplicationConnections = comV77ApplicationConnectionStatuses,
            V77ApplicationPeriodProduceJobs = v77ApplicationPeriodProduceJobStatuses,
        }, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }

    private static void AddProducerStatuses(ProducerStatusesGatherer gatherer, HealthReport healthReport, ref List<LegacyProducerHealthStatus> statuses)
    {
        List<LegacyProducerHealthStatus>? gatheredStatuses = gatherer(healthReport);

        if (gatheredStatuses is not null)
        {
            statuses.AddRange(gatheredStatuses);
        }
    }

    private static void AddConsumerStatuses(ConsumerStatusesGatherer gatherer, HealthReport healthReport, ref List<LegacyConsumerHealthStatus> statuses)
    {
        List<LegacyConsumerHealthStatus>? gatheredStatuses = gatherer(healthReport);

        if (gatheredStatuses is not null)
        {
            statuses.AddRange(gatheredStatuses);
        }
    }

    private static List<LegacyConsumerHealthStatus>? GetMsSqlConsumerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<MsSqlConsumerStatus>? statuses = GetStatusFromHealthReport<MsSqlConsumerStatus>(
            healthReport,
            dataKey: MsSqlConsumerServiceHealthChecker.DataKey);

        if (statuses is null)
        {
            return null;
        }

        List<LegacyConsumerHealthStatus> oldStatuses = [];

        foreach (MsSqlConsumerStatus status in statuses)
        {
            oldStatuses.Add(new LegacyConsumerHealthStatus()
            {
                Type = nameof(MsSqlConsumerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                Consumed = status.Consumed,
                Saved = status.Saved,
                TableJsonProperty = status.TablePropertyName,
                Topics = [.. status.Topics],
                ConsumerGroup = status.ConsumerGroup,
                SuspendSchedule = status.SuspendSchedule,
            });
        }

        return oldStatuses;
    }

    private static List<LegacyProducerHealthStatus>? GetV77ApplicationProducerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V77ApplicationProducerStatus>? statuses = GetStatusFromHealthReport<V77ApplicationProducerStatus>(
            healthReport,
            dataKey: V77ApplicationProducerServiceHealthChecker.DataKey);

        if (statuses is null)
        {
            return null;
        }

        List<LegacyProducerHealthStatus> oldStatuses = [];

        foreach (V77ApplicationProducerStatus status in statuses)
        {
            oldStatuses.Add(new LegacyProducerHealthStatus()
            {
                Type = nameof(V77ApplicationProducerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                ObjectFilters = [.. status.ObjectFilters.Select(f => $"{f.Id}:{f.Depth}")],
                TransactionTypes = [.. status.TransactionTypeFilters],
                ReadFromLogFile = status.GotLogTransactions,
                Fetched = status.Fetched,
                Produced = status.Produced,
                InfobasePath = status.InfobasePath,
                Username = status.Username,
                DataTypeJsonProperty = status.DataTypePropertyName,
                SuspendSchedule = status.SuspendSchedule,
            });
        }

        return oldStatuses;
    }

    private static List<LegacyConsumerHealthStatus>? GetV77ApplicationConsumerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V77ApplicationConsumerStatus>? statuses = GetStatusFromHealthReport<V77ApplicationConsumerStatus>(
            healthReport,
            dataKey: V77ApplicationConsumerServiceHealthChecker.DataKey);

        if (statuses is null)
        {
            return null;
        }

        List<LegacyConsumerHealthStatus> oldStatuses = [];

        foreach (V77ApplicationConsumerStatus status in statuses)
        {
            oldStatuses.Add(new LegacyConsumerHealthStatus()
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
                SuspendSchedule = status.SuspendSchedule,
            });
        }

        return oldStatuses;
    }

    private static List<LegacyProducerHealthStatus>? GetV83ApplicationProducerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V83ApplicationProducerStatus>? statuses = GetStatusFromHealthReport<V83ApplicationProducerStatus>(
            healthReport,
            dataKey: V83ApplicationProducerServiceHealthChecker.DataKey);

        if (statuses is null)
        {
            return null;
        }

        List<LegacyProducerHealthStatus> oldStatuses = [];

        foreach (V83ApplicationProducerStatus status in statuses)
        {
            oldStatuses.Add(new LegacyProducerHealthStatus()
            {
                Type = nameof(V83ApplicationProducerService),
                Active = status.Active,
                LastActivity = status.LastActivity,
                ErrorMessage = status.ErrorMessage,
                ObjectFilters = [.. status.ObjectFilters],
                TransactionTypes = [.. status.TransactionTypeFilters],
                Fetched = status.Fetched,
                Produced = status.Produced,
                InfobasePath = status.InfobaseUrl,
                Username = status.Username,
                DataTypeJsonProperty = status.DataTypePropertyName,
                SuspendSchedule = status.SuspendSchedule,

                // Unused
                ReadFromLogFile = null,
            });
        }

        return oldStatuses;
    }

    private static List<LegacyConsumerHealthStatus>? GetV83ApplicationConsumerStatuses(HealthReport healthReport)
    {
        IReadOnlyList<V83ApplicationConsumerStatus>? statuses = GetStatusFromHealthReport<V83ApplicationConsumerStatus>(
            healthReport,
            dataKey: V83ApplicationConsumerServiceHealthChecker.DataKey);

        if (statuses is null)
        {
            return null;
        }

        List<LegacyConsumerHealthStatus> oldStatuses = [];

        foreach (V83ApplicationConsumerStatus status in statuses)
        {
            oldStatuses.Add(new LegacyConsumerHealthStatus()
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
                SuspendSchedule = status.SuspendSchedule,
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

        if (serviceStatus.Statuses is null || serviceStatus.Statuses.Count == 0)
        {
            return null;
        }

        return serviceStatus.Statuses;
    }

    private static List<ComV77ApplicationConnectionHealthStatus>? GetComV77ApplicationConnectionHealthStatuses(HealthReport healthReport)
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
            .Select(s => new ComV77ApplicationConnectionHealthStatus()
            {
                InfobasePath = s.InfobasePath,
                Username = s.Username,
                ErrorsCount = s.ErrorsCount,
                RetrievedTimes = s.RetrievedTimes,
                IsInitialized = s.IsInitialized,
                IsDisposed = s.IsDisposed,
                LastTimeDisposed = s.LastTimeDisposed,
            })
            .ToList();
    }

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

    private delegate List<LegacyProducerHealthStatus>? ProducerStatusesGatherer(HealthReport healthReport);

    private delegate List<LegacyConsumerHealthStatus>? ConsumerStatusesGatherer(HealthReport healthReport);
}
