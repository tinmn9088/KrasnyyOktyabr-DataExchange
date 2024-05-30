#nullable enable

using System.Collections.Concurrent;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using static KrasnyyOktyabr.ApplicationNet48.Services.IJsonService;
using static KrasnyyOktyabr.ApplicationNet48.Services.IV77ApplicationLogService;
using static KrasnyyOktyabr.ApplicationNet48.Services.Kafka.V77ApplicationHelper;
using static KrasnyyOktyabr.ApplicationNet48.Services.V77ApplicationLogService;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using KrasnyyOktyabr.ApplicationNet48.Logging;
using System.IO;
using System.Linq;
using System;
using KrasnyyOktyabr.ApplicationNet48.Linq;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public sealed class V77ApplicationPeriodProduceJobService(
    ILogger<V77ApplicationProducerService> logger,
    ILoggerFactory loggerFactory,
    IV77ApplicationLogService logService,
    IComV77ApplicationConnectionFactory connectionFactory,
    IJsonService jsonService,
    IKafkaService kafkaService)
    : IV77ApplicationPeriodProduceJobService
{
    public delegate ValueTask<GetLogTransactionsResult> ProduceFromPeriodAsync(
        ILogger logger,
        V77ApplicationPeriodProduceJobRequest request,
        List<ObjectFilter> objectFilters,
        IV77ApplicationLogService logService,
        CancellationToken cancellationToken);

    public delegate ValueTask<List<string>> GetObjectJsonsAsync(
        ILogger logger,
        V77ApplicationPeriodProduceJobRequest request,
        LogTransaction[] logTransactionsChunk,
        List<ObjectFilter> objectFilters,
        IComV77ApplicationConnectionFactory connectionFactory,
        CancellationToken cancellationToken);

    /// <returns>Sent objects count.</returns>
    public delegate ValueTask<int> SendObjectJsonsAsync(
        ILogger logger,
        V77ApplicationPeriodProduceJobRequest request,
        LogTransaction[] logTransactionsChunk,
        List<string> objectJsons,
        IJsonService jsonService,
        IKafkaService kafkaService,
        CancellationToken cancellationToken);

    private static int LogTransactionChunkSize => 10;

    public List<V77ApplicationPeriodProduceJobStatus> Status
    {
        get
        {
            List<V77ApplicationPeriodProduceJobStatus> jobStatuses = [];

            foreach (V77ApplicationPeriodProduceJob job in _jobs.Values)
            {
                jobStatuses.Add(new()
                {
                    LastActivity = job.LastActivity,
                    IsCancellationRequested = job.IsCancellationRequested,
                    ErrorMessage = job.Error?.Message,
                    InfobasePath = job.InfobasePath,
                    Username = job.Username,
                    FoundLogTransactions = job.FoundLogTransactions,
                    Fetched = job.Fetched,
                    Produced = job.Produced,
                    ObjectFilters = job.ObjectFilters,
                    TransactionTypeFilters = job.TransactionTypeFilters,
                    DataTypePropertyName = job.DataTypePropertyName,
                });
            }

            return jobStatuses;
        }
    }

    public ProduceFromPeriodAsync ProduceFromPeriodTask => async (
            ILogger logger,
            V77ApplicationPeriodProduceJobRequest request,
            List<ObjectFilter> objectFilters,
            IV77ApplicationLogService logService,
            CancellationToken cancellationToken) =>
    {
        string infobaseFullPath = GetInfobaseFullPath(request.InfobasePath);

        logger.LogGettingTransactionsForPeriod(infobaseFullPath, request.Start, request.Start + request.Duration);

        TransactionFilter filter = new(
            objectIds: objectFilters.Select(f => f.Id).ToArray(),
            transactionTypes: request.TransactionTypeFilters
        );

        string logFilePath = Path.Combine(infobaseFullPath, LogFileRelativePath);

        return await logService.GetLogTransactionsForPeriodAsync(
            logFilePath,
            filter,
            start: request.Start,
            duration: request.Duration,
            cancellationToken).ConfigureAwait(false);
    };

    public GetObjectJsonsAsync GetObjectJsonsTask => async (
        ILogger logger,
        V77ApplicationPeriodProduceJobRequest request,
        LogTransaction[] logTransactionsChunk,
        List<ObjectFilter> objectFilters,
        IComV77ApplicationConnectionFactory connectionFactory,
        CancellationToken cancellationToken) =>
    {
        if (logTransactionsChunk.Length == 0)
        {
            return [];
        }

        string infobaseFullPath = GetInfobaseFullPath(request.InfobasePath);

        logger.LogGettingObjectsFromInfobase(infobaseFullPath);

        List<string> objectIds = logTransactionsChunk.Select(t => t.ObjectId).ToList();

        ConnectionProperties connectionProperties = new(
            infobasePath: infobaseFullPath,
            username: request.Username,
            password: request.Password
        );

        List<string> objectJsons = new(objectIds.Count);

        await using (IComV77ApplicationConnection connection = await connectionFactory.GetConnectionAsync(connectionProperties, cancellationToken).ConfigureAwait(false))
        {
            await connection.ConnectAsync(cancellationToken);

            foreach (string objectId in objectIds)
            {
                int depth = objectFilters
                .Where(f => objectId.StartsWith(f.Id))
                .Select(f => f.Depth)
                .FirstOrDefault();

                if (depth == default)
                {
                    depth = ObjectFilterDefaultDepth;
                }

                // Prepare ERT parameters
                Dictionary<string, string> ertContext = new()
                {
                    { "ObjectId", objectId },
                    { "JsonMaxDepth", depth.ToString() },
                    { "DatabaseGUIDsConnectionString", request.DocumentGuidsDatabaseConnectionString ?? string.Empty },
                };

                object? result = await connection.RunErtAsync(
                    ertRelativePath: GetErtRelativePath(request),
                    ertContext,
                    resultName: "ObjectJSON",
                    cancellationToken).ConfigureAwait(false);

                if (result == null || result.ToString() == "null")
                {
                    throw new FailedToGetObjectException(objectId);
                }

                objectJsons.Add(result.ToString() ?? throw new FailedToGetObjectException(objectId));
            }
        }

        logger.LogGotObjectsFromInfobase(objectJsons.Count, infobaseFullPath);

        return objectJsons;
    };

    public SendObjectJsonsAsync SendObjectJsonsTask => async (
        ILogger logger,
        V77ApplicationPeriodProduceJobRequest request,
        LogTransaction[] logTransactionsChunk,
        List<string> objectJsons,
        IJsonService jsonService,
        IKafkaService kafkaService,
        CancellationToken cancellationToken) =>
    {
        List<KafkaProducerMessageData> messagesData = new(objectJsons.Count);

        for (int i = 0, count = objectJsons.Count; i < count; i++)
        {
            string objectDateString = ObjectDateRegex.Match(logTransactionsChunk[i].ObjectName).Groups[1].Value;

            Dictionary<string, object?> propertiesToAdd = new()
            {
                { TransactionTypePropertyName, logTransactionsChunk[i].Type },
                { ObjectDatePropertyName, objectDateString },
            };

            KafkaProducerMessageData messageData = jsonService.BuildKafkaProducerMessageData(objectJsons[i], propertiesToAdd, request.DataTypePropertyName);

            messagesData.Add(messageData);
        }

        logger.LogSendingObjects(messagesData.Count);

        using IProducer<string, string> producer = kafkaService.GetProducer<string, string>();

        string infobasePubName = GetInfobasePubName(request.InfobasePath);

        int sentObjectsCount = 0;

        foreach (KafkaProducerMessageData messageData in messagesData)
        {
            Message<string, string> kafkaMessage = new()
            {
                Key = infobasePubName,
                Value = messageData.ObjectJson,
            };

            string topicName = kafkaService.BuildTopicName(infobasePubName, messageData.DataType);

            await producer.ProduceAsync(topicName, kafkaMessage, cancellationToken);

            logger.LogProducedMessage(topicName, kafkaMessage.Key, kafkaMessage.Value);

            sentObjectsCount++;
        }

        return sentObjectsCount;
    };

    /// <summary>
    /// Keys are infobase paths.
    /// </summary>
    private readonly ConcurrentDictionary<string, V77ApplicationPeriodProduceJob> _jobs = [];

    /// <exception cref="FailedToStartJob"></exception>
    public void StartJob(V77ApplicationPeriodProduceJobRequest request)
    {
        string key = request.InfobasePath;

        if (_jobs.TryAdd(key, new(
                logger: loggerFactory.CreateLogger<V77ApplicationPeriodProduceJob>(),
                request,
                logService,
                connectionFactory,
                jsonService,
                kafkaService,
                ProduceFromPeriodTask,
                GetObjectJsonsTask,
                SendObjectJsonsTask,
                removeFromJobsCallback: () => _jobs.TryRemove(key, out V77ApplicationPeriodProduceJob? _))))
        {
            logger.LogStartingPeriodProduceJob(request.InfobasePath, request.Start, request.Start + request.Duration);
        }
        else
        {
            throw new FailedToStartJob(request.InfobasePath);
        }
    }

    public async ValueTask CancelJobAsync(string infobasePath)
    {
        if (_jobs.TryRemove(infobasePath, out V77ApplicationPeriodProduceJob? job))
        {
            logger.LogCancellingPeriodProduceJob(infobasePath, job.Start, job.End);

            await job.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogDisposing();

        int disposedCount = 0;

        foreach (V77ApplicationPeriodProduceJob job in _jobs.Values)
        {
            await job.DisposeAsync();

            disposedCount++;
        }

        if (disposedCount > 0)
        {
            logger.LogDisposedPeriodProduceJobs(disposedCount);
        }
    }

    private class V77ApplicationPeriodProduceJob : IAsyncDisposable
    {
        private readonly ILogger<V77ApplicationPeriodProduceJob> _logger;

        private readonly V77ApplicationPeriodProduceJobRequest _request;

        private readonly List<ObjectFilter> _objectFilters;

        private readonly IV77ApplicationLogService _logService;

        private readonly IComV77ApplicationConnectionFactory _connectionFactory;

        private readonly IJsonService _jsonService;

        private readonly IKafkaService _kafkaService;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly ProduceFromPeriodAsync _produceFromPeriodTask;

        private readonly GetObjectJsonsAsync _getObjectJsonsTask;

        private readonly SendObjectJsonsAsync _sendObjectJsonsTask;

        private readonly Action _removeFromJobsCallback;

        private readonly Task _produceTask;

        internal V77ApplicationPeriodProduceJob(
            ILogger<V77ApplicationPeriodProduceJob> logger,
            V77ApplicationPeriodProduceJobRequest request,
            IV77ApplicationLogService logService,
            IComV77ApplicationConnectionFactory connectionFactory,
            IJsonService jsonService,
            IKafkaService kafkaService,
            ProduceFromPeriodAsync produceFromPeriodTask,
            GetObjectJsonsAsync getObjectJsonsTask,
            SendObjectJsonsAsync sendObjectJsonsTask,
            Action removeFromJobsCallback)
        {
            _logger = logger;
            _request = request;
            _logService = logService;
            _connectionFactory = connectionFactory;
            _jsonService = jsonService;
            _kafkaService = kafkaService;

            _objectFilters = GetObjectFilters(request);

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _produceFromPeriodTask = produceFromPeriodTask;
            _getObjectJsonsTask = getObjectJsonsTask;
            _sendObjectJsonsTask = sendObjectJsonsTask;
            _removeFromJobsCallback = removeFromJobsCallback;

            _produceTask = Task.Run(() => RunAsync(cancellationToken));
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _logger.LogDisposingPeriodProduceJob(InfobasePath, Start, End);

            await _produceTask.ConfigureAwait(false);

            _logger.LogDisposedPeriodProduceJob(InfobasePath, Start, End);
        }

        public bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public string DataTypePropertyName => _request.DataTypePropertyName;

        public string InfobasePath => _request.InfobasePath;

        public string Username => _request.Username;

        public DateTimeOffset Start => _request.Start;

        public DateTimeOffset End => _request.Start + _request.Duration;

        public string[] TransactionTypeFilters => _request.TransactionTypeFilters;

        public IReadOnlyList<ObjectFilter> ObjectFilters => _objectFilters.AsReadOnly();

        public Exception? Error { get; private set; }

        public int FoundLogTransactions { get; private set; } = 0;

        public int Fetched { get; private set; } = 0;

        public int Produced { get; private set; } = 0;

        public DateTimeOffset LastActivity { get; private set; }

        private async ValueTask RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                await Task.Delay(TimeSpan.FromSeconds(2)); // Prevent spamming start requests

                GetLogTransactionsResult getLogTransactionsResult = await _produceFromPeriodTask(
                    _logger,
                    _request,
                    _objectFilters,
                    _logService,
                    cancellationToken)
                    .ConfigureAwait(false);

                List<LogTransaction> logTransactions = getLogTransactionsResult.Transactions;

                FoundLogTransactions = logTransactions.Count;

                cancellationToken.ThrowIfCancellationRequested();

                foreach (LogTransaction[] logTransactionsChunk in logTransactions.Chunk(LogTransactionChunkSize))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    List<string> objectJsons = await _getObjectJsonsTask(
                        _logger,
                        _request,
                        logTransactionsChunk,
                        _objectFilters,
                        _connectionFactory,
                        cancellationToken)
                        .ConfigureAwait(false);

                    Fetched += objectJsons.Count;

                    cancellationToken.ThrowIfCancellationRequested();

                    int sentObjectsCount = await _sendObjectJsonsTask(
                        _logger,
                        _request,
                        logTransactionsChunk,
                        objectJsons,
                        _jsonService,
                        _kafkaService,
                        cancellationToken)
                        .ConfigureAwait(false);

                    Produced += sentObjectsCount;
                }

                _removeFromJobsCallback.Invoke();

                _logger.LogPeriodProduceJobFinished(InfobasePath, Start, End);
            }
            catch (OperationCanceledException)
            {
                _logger.LogCancelledPeriodProduceJob(InfobasePath, Start, End);
            }
            catch (Exception ex)
            {
                Error = ex;

                _logger.LogPeriodProduceJobError(ex, InfobasePath, Start, End);
            }
        }
    }

    private static string GetInfobaseFullPath(string infobasePath) => Path.GetFullPath(infobasePath);

    private static string GetInfobasePubName(string infobasePath) => Path.GetFileName(infobasePath);

    private static string GetErtRelativePath(V77ApplicationPeriodProduceJobRequest request) => request.ErtRelativePath ?? DefaultErtRelativePath;

    private static List<ObjectFilter> GetObjectFilters(V77ApplicationPeriodProduceJobRequest request)
    {
        return request.ObjectFilters
            .Select(f => f.Split(ObjectFilterValuesSeparator))
            .Select(splitted => new ObjectFilter(
                id: splitted[0],
                depth: int.TryParse(splitted[1], out int depth) ? depth : ObjectFilterDefaultDepth
            ))
            .ToList();
    }

    private class FailedToStartJob(string infobasePath)
        : Exception($"Failed to start job on '{infobasePath}' (probably one is already running)")
    {
    }
}
