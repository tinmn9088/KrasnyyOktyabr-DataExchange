using System.Collections.Concurrent;
using System.Runtime.Versioning;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using KrasnyyOktyabr.Application.Logging;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using static KrasnyyOktyabr.Application.Services.IJsonService;
using static KrasnyyOktyabr.Application.Services.IV77ApplicationLogService;
using static KrasnyyOktyabr.Application.Services.Kafka.V77ApplicationProducersHelper;

namespace KrasnyyOktyabr.Application.Services.Kafka;

[SupportedOSPlatform("windows")]
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

        logger.GettingTransactionsForPeriod(infobaseFullPath, request.Start, request.Start + request.Duration);

        TransactionFilter filter = new()
        {
            ObjectIds = objectFilters.Select(f => f.Id).ToArray(),
            TransactionTypes = request.TransactionTypeFilters,
        };

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

        logger.GettingObjects(infobaseFullPath);

        List<string> objectIds = logTransactionsChunk.Select(t => t.ObjectId).ToList();

        ConnectionProperties connectionProperties = new()
        {
            InfobasePath = infobaseFullPath,
            Username = request.Username,
            Password = request.Password,
        };

        List<string> objectJsons = new(objectIds.Count);

        await using (IComV77ApplicationConnection connection = await connectionFactory.GetConnectionAsync(connectionProperties, cancellationToken).ConfigureAwait(false))
        {
            await connection.ConnectAsync(cancellationToken);

            foreach (string objectId in objectIds)
            {
                int depth = objectFilters
                .Where(f => f.Id == objectId)
                .Select(f => f.Depth)
                .FirstOrDefault(ObjectFilterDefaultDepth);

                // Prepare ERT parameters
                Dictionary<string, object?> ertContext = new()
                {
                    { "ObjectId", objectId },
                    { "JsonMaxDepth", depth },
                    { "DatabaseGUIDsConnectionString", request.DocumentGuidsDatabaseConnectionString },
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

        logger.GotObjects(objectJsons.Count);

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
            string objectDateString = ObjectDateRegex().Match(logTransactionsChunk[i].ObjectName).Groups[1].Value;

            Dictionary<string, object?> propertiesToAdd = new()
            {
                { TransactionTypePropertyName, logTransactionsChunk[i].Type },
                { ObjectDatePropertyName, objectDateString },
            };

            KafkaProducerMessageData messageData = jsonService.BuildKafkaProducerMessageData(objectJsons[i], propertiesToAdd, request.DataTypePropertyName);

            messagesData.Add(messageData);
        }

        logger.SendingObjectJsons(messagesData.Count);

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
                removeFromJobsCallback: () => _jobs.Remove(key, out V77ApplicationPeriodProduceJob? _))))
        {
            logger.StartingPeriodProduceJob(request.InfobasePath, request.Start, request.Start + request.Duration);
        }
        else
        {
            throw new FailedToStartJob(request.InfobasePath);
        }
    }

    public async ValueTask CancelJobAsync(string infobasePath)
    {
        if (_jobs.Remove(infobasePath, out V77ApplicationPeriodProduceJob? job))
        {
            logger.CancellingPeriodProduceJob(infobasePath, job.Start, job.End);

            await job.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogTrace("Disposing ...");

        int disposedCount = 0;

        foreach (V77ApplicationPeriodProduceJob job in _jobs.Values)
        {
            await job.DisposeAsync();

            disposedCount++;
        }

        if (disposedCount > 0)
        {
            logger.DisposedProduceJobs(disposedCount);
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

            _logger.DisposingProduceJob(InfobasePath, Start, End);

            await _produceTask.ConfigureAwait(false);
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

                _logger.PeriodProduceJobFinished(InfobasePath, Start, End);
            }
            catch (OperationCanceledException)
            {
                _logger.PeriodProduceJobCancelled(InfobasePath, Start, End);
            }
            catch (Exception ex)
            {
                Error = ex;

                _logger.ErrorOnPeriodProduceJob(ex, InfobasePath, Start, End);
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
            .Select(splitted => new ObjectFilter()
            {
                Id = splitted[0],
                Depth = int.TryParse(splitted[1], out int depth) ? depth : ObjectFilterDefaultDepth
            })
            .ToList();
    }

    private class FailedToStartJob(string infobasePath)
        : Exception($"Failed to start job on '{infobasePath}' (probably one is already running)")
    {
    }
}
