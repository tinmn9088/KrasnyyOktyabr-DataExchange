using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.Application.Logging;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using static KrasnyyOktyabr.Application.Services.IJsonService;
using static KrasnyyOktyabr.Application.Services.IV77ApplicationLogService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

[SupportedOSPlatform("windows")]
public sealed partial class V77ApplicationProducerService(
    IConfiguration configuration,
    ILogger<V77ApplicationProducerService> logger,
    ILoggerFactory loggerFactory,
    IOffsetService offsetService,
    IV77ApplicationLogService logService,
    IComV77ApplicationConnectionFactory connectionFactory,
    IJsonService jsonService,
    IKafkaService kafkaService)
    : IV77ApplicationProducerService
{
    public readonly struct ObjectFilter
    {
        public required string Id { get; init; }

        public required int Depth { get; init; }
    }

    public delegate Task<GetLogTransactionsResult> GetLogTransactionsAsync(
        V77ApplicationProducerSettings settings,
        IOffsetService offsetService,
        IV77ApplicationLogService logService,
        ILogger logger,
        CancellationToken cancellationToken);

    public delegate Task<List<string>> GetObjectJsonsAsync(
        V77ApplicationProducerSettings settings,
        List<LogTransaction> logTransactions,
        List<ObjectFilter> objectFilters,
        IComV77ApplicationConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken);

    public delegate Task SendObjectJsonsAsync(
        V77ApplicationProducerSettings settings,
        List<LogTransaction> logTransactions,
        List<string> objectJsons,
        IJsonService jsonService,
        IKafkaService kafkaService,
        CancellationToken cancellationToken);

    public static string LogFileRelativePath => @"SYSLOG\1cv7.mlg";

    public static string DefaultErtRelativePath => @"ExtForms\EDO\Test\UN_JSON_Synch.ert";

    public static string TransactionTypePropertyName => "ТипТранзакции";

    public static string ObjectDatePropertyName => "ДатаДокИзЛогов";

    private static char OffsetValuesSeparator => '&';

    private static char ObjectFilterValuesSeparator => ':';

    private static int ObjectFilterDefaultDepth => 1;

    /// <summary>
    /// <para>
    /// Is <c>null</c> when no configuration found.
    /// </para>
    /// <para>
    /// Keys are results of <see cref="V77ApplicationProducer.Key"/>.
    /// </para>
    /// </summary>
    private Dictionary<string, V77ApplicationProducer>? _producers;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Starting ...");

        StartProducers();

        return Task.CompletedTask;
    }

    public async Task RestartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Restarting ...");

        await StopProducersAsync();

        StartProducers();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Stopping ...");

        await StopProducersAsync();
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogTrace("Disposing ...");

        await StopProducersAsync();
    }

    public GetLogTransactionsAsync GetLogTransactionsTask => async (
    V77ApplicationProducerSettings settings,
    IOffsetService offsetService,
    IV77ApplicationLogService logService,
    ILogger logger,
    CancellationToken cancellationToken) =>
    {
        string infobaseFullPath = GetInfobaseFullPath(settings.InfobasePath);

        logger.GettingNewTransactions(infobaseFullPath);

        LogOffset commitedOffset = await GetCommitedOffset(offsetService, infobaseFullPath, logger, cancellationToken).ConfigureAwait(false);

        TransactionFilter filter = new()
        {
            SeekBackPosition = commitedOffset.Position,
            CommittedLine = commitedOffset.LastReadLine,
            ObjectIds = settings.ObjectFilters.Select(f => f.Split(ObjectFilterValuesSeparator, 2)[0]).ToArray(),
            TransactionTypes = settings.TransactionTypeFilters,
        };

        string logFilePath = Path.Combine(infobaseFullPath, LogFileRelativePath);

        return await logService.GetLogTransactions(logFilePath, filter, cancellationToken).ConfigureAwait(false);
    };

    public GetObjectJsonsAsync GetObjectJsonsTask => async (
        V77ApplicationProducerSettings settings,
        List<LogTransaction> logTransactions,
        List<ObjectFilter> objectFilters,
        IComV77ApplicationConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken) =>
    {
        if (logTransactions.Count == 0)
        {
            return [];
        }

        string infobaseFullPath = GetInfobaseFullPath(settings.InfobasePath);

        logger.GettingObjects(infobaseFullPath);

        List<string> objectIds = logTransactions.Select(t => t.ObjectId).ToList();

        ConnectionProperties connectionProperties = new()
        {
            InfobasePath = infobaseFullPath,
            Username = settings.Username,
            Password = settings.Password,
        };

        List<string> objects = new(objectIds.Count);

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
                    { "DatabaseGUIDsConnectionString", settings.DocumentGuidsDatabaseConnectionString },
                };

                object? result = await connection.RunErtAsync(
                    ertRelativePath: GetErtRelativePath(settings),
                    ertContext,
                    resultName: "ObjectJSON",
                    cancellationToken).ConfigureAwait(false);

                if (result == null || result.ToString() == "null")
                {
                    throw new FailedToGetObjectException(objectId);
                }

                objects.Add(result.ToString() ?? throw new FailedToGetObjectException(objectId));
            }
        }

        logger.GotObjects(objects.Count);

        return objects;
    };

    public SendObjectJsonsAsync SendObjectJsonsTask => async (
        V77ApplicationProducerSettings settings,
        List<LogTransaction> logTransactions,
        List<string> objectJsons,
        IJsonService jsonService,
        IKafkaService kafkaService,
        CancellationToken cancellationToken) =>
    {
        if (logTransactions.Count != objectJsons.Count)
        {
            throw new ArgumentException($"Transactions count ({logTransactions.Count}) != Object JSONs count ({objectJsons.Count})");
        }

        List<V77ApplicationProducerMessageData> messagesData = new(objectJsons.Count);

        for (int i = 0, count = objectJsons.Count; i < count; i++)
        {
            string objectDateString = ObjectDateRegex().Match(logTransactions[i].ObjectName).Groups[1].Value;

            Dictionary<string, object?> propertiesToAdd = new()
            {
                { TransactionTypePropertyName, logTransactions[i].Type },
                { ObjectDatePropertyName, objectDateString },
            };

            V77ApplicationProducerMessageData messageData = jsonService.BuildV77ApplicationProducerMessageData(objectJsons[i], propertiesToAdd, settings.DataTypeJsonPropertyName);

            messagesData.Add(messageData);
        }

        logger.SendingObjectJsons(messagesData.Count);

        using IProducer<string, string> producer = kafkaService.GetProducer<string, string>();

        string infobasePubName = GetInfobasePubName(settings.InfobasePath);

        foreach (V77ApplicationProducerMessageData messageData in messagesData)
        {
            Message<string, string> kafkaMessage = new()
            {
                Key = infobasePubName,
                Value = messageData.ObjectJson,
            };

            string topicName = kafkaService.BuildTopicName(infobasePubName, messageData.DataType);

            await producer.ProduceAsync(topicName, kafkaMessage, cancellationToken);
        }
    };

    /// <summary>
    /// Calls <see cref="StartProducer(V77ApplicationProducerSettings)"/> using all settings found.
    /// </summary>
    private void StartProducers()
    {
        V77ApplicationProducerSettings[]? producersSettings = GetProducersSettings();

        if (producersSettings == null)
        {
            logger.ConfigurationNotFound();

            _producers = null;

            return;
        }

        logger.ConfigurationFound(producersSettings.Length);

        foreach (V77ApplicationProducerSettings settings in producersSettings)
        {
            StartProducer(settings);
        }
    }

    private V77ApplicationProducerSettings[]? GetProducersSettings()
    {
        return configuration
            .GetSection(V77ApplicationProducerSettings.Position)
            .Get<V77ApplicationProducerSettings[]>();
    }

    /// <summary>
    /// Creates new <see cref="V77ApplicationProducer"/> and saves it to <see cref="_producers"/>.
    /// </summary>
    private void StartProducer(V77ApplicationProducerSettings settings)
    {
        _producers ??= [];

        V77ApplicationProducer producer = new(
            settings,
            logger: loggerFactory.CreateLogger<V77ApplicationProducer>(),
            logService,
            offsetService,
            connectionFactory,
            jsonService,
            kafkaService,
            GetLogTransactionsTask,
            GetObjectJsonsTask,
            SendObjectJsonsTask);

        _producers.Add(producer.Key, producer);
    }

    private async Task StopProducersAsync()
    {
        if (_producers != null)
        {
            logger.StoppingProducers(_producers.Count);

            foreach (V77ApplicationProducer producer in _producers.Values)
            {
                await producer.DisposeAsync();
            }

            _producers?.Clear();
        }
    }

    private partial class V77ApplicationProducer : IAsyncDisposable
    {
        private static TimeSpan MinChangesInterval => TimeSpan.FromSeconds(2);

        private readonly ILogger<V77ApplicationProducer> _logger;

        private readonly V77ApplicationProducerSettings _settings;

        private readonly string _infobaseFullPath;

        private readonly IV77ApplicationLogService _logService;

        private readonly IOffsetService _offsetService;

        private readonly IComV77ApplicationConnectionFactory _connectionFactory;

        private readonly IJsonService _jsonService;

        private readonly IKafkaService _kafkaService;

        private readonly List<ObjectFilter> _objectFilters;

        /// <remarks>
        /// Has to be disposed.
        /// </remarks>
        private readonly FileSystemWatcher _watcher;

        private readonly SemaphoreSlim _watcherHandlersLock;

        private readonly SemaphoreSlim _processingLock;

        private readonly GetLogTransactionsAsync _getLogTransactionsTask;

        private readonly GetObjectJsonsAsync _getObjectJsonsTask;

        private readonly SendObjectJsonsAsync _sendObjectJsonsTask;

        /// <remarks>
        /// Has to be disposed.
        /// </remarks>
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task? _currentProcessingTask;

        public V77ApplicationProducer(
            V77ApplicationProducerSettings settings,
            ILogger<V77ApplicationProducer> logger,
            IV77ApplicationLogService logService,
            IOffsetService offsetService,
            IComV77ApplicationConnectionFactory connectionFactory,
            IJsonService jsonService,
            IKafkaService kafkaService,
            GetLogTransactionsAsync getLogTransactionsTask,
            GetObjectJsonsAsync getObjectJsonsTask,
            SendObjectJsonsAsync sendObjectJsonsTask)
        {
            _logger = logger;
            _settings = settings;
            _logService = logService;
            _offsetService = offsetService;
            _connectionFactory = connectionFactory;
            _jsonService = jsonService;
            _kafkaService = kafkaService;
            _objectFilters = GetObjectFilters(settings);

            _infobaseFullPath = GetInfobaseFullPath(settings.InfobasePath);

            _logger.StartingWatchingChanges(_infobaseFullPath);

            _watcher = new()
            {
                Path = _infobaseFullPath,
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
            };

            _watcher.Changed += new FileSystemEventHandler(OnInfobaseChange);

            _watcher.EnableRaisingEvents = true;

            // First handler enters both locks, the next one enters only the first lock:
            // it serves as a signal that there were changes after the first handler started processing.
            _watcherHandlersLock = new(2, 2);
            _processingLock = new(1, 1);

            _getLogTransactionsTask = getLogTransactionsTask;
            _getObjectJsonsTask = getObjectJsonsTask;
            _sendObjectJsonsTask = sendObjectJsonsTask;

            _currentProcessingTask = null;
            _cancellationTokenSource = new();
        }

        public string Key => _infobaseFullPath;

        public Task? CurrentProcessing => _currentProcessingTask;

        public async ValueTask DisposeAsync()
        {
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            if (_currentProcessingTask != null)
            {
                await _currentProcessingTask.WaitAsync(CancellationToken.None);
            }
        }

        private void OnInfobaseChange(object sender, FileSystemEventArgs e)
        {
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            if (_watcherHandlersLock.Wait(0))
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await _processingLock.WaitAsync(cancellationToken).ConfigureAwait(false);

                        try
                        {
                            _logger.ProcessingInfobaseChange(_infobaseFullPath);

                            await Task.Delay(MinChangesInterval, cancellationToken).ConfigureAwait(false);

                            _currentProcessingTask = ProcessChanges(cancellationToken);

                            await _currentProcessingTask.ConfigureAwait(false);
                        }
                        finally
                        {
                            _processingLock.Release();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        _logger.OperationCancelled();
                    }
                    catch (Exception ex)
                    {
                        _logger.ErrorOnInfobaseChanges(ex, ex);
                    }
                    finally
                    {
                        _watcherHandlersLock.Release();
                    }
                }, cancellationToken);
            }
        }

        /// <remarks>
        /// Triggered by <see cref="_watcher"/> events with <see cref="MinChangesInterval"/>.
        /// </remarks>
        private async Task ProcessChanges(CancellationToken cancellationToken)
        {
            GetLogTransactionsResult getLogTransactionsResult = await _getLogTransactionsTask(
                _settings,
                _offsetService,
                _logService,
                _logger,
                cancellationToken);

            if (getLogTransactionsResult.Transactions.Count == 0)
            {
                return;
            }

            List<string> objectJsons = await _getObjectJsonsTask(
                _settings,
                getLogTransactionsResult.Transactions,
                _objectFilters,
                _connectionFactory,
                _logger,
                cancellationToken);

            await _sendObjectJsonsTask(
                _settings,
                getLogTransactionsResult.Transactions,
                objectJsons,
                _jsonService,
                _kafkaService,
                cancellationToken);

            await CommitOffset(
                _offsetService,
                _infobaseFullPath,
                position: getLogTransactionsResult.LastReadOffset.Position,
                lastReadLine: getLogTransactionsResult.LastReadOffset.LastReadLine,
                cancellationToken);
        }
    }

    private static string GetInfobaseFullPath(string infobasePath) => Path.GetFullPath(infobasePath);

    private static string GetInfobasePubName(string infobasePath) => Path.GetFileName(infobasePath);

    private static string GetErtRelativePath(V77ApplicationProducerSettings settings) => settings.ErtRelativePath ?? DefaultErtRelativePath;

    private static async Task<LogOffset> GetCommitedOffset(IOffsetService offsetService, string infobaseFullPath, ILogger logger, CancellationToken cancellationToken)
    {
        string? commitedOffsetString = await offsetService.GetOffset(infobaseFullPath, cancellationToken).ConfigureAwait(false);

        if (commitedOffsetString == null)
        {
            return new()
            {
                Position = null,
                LastReadLine = string.Empty,
            };
        }

        string[] positionAndLineStrings = commitedOffsetString.Split(OffsetValuesSeparator, 2);

        if (positionAndLineStrings.Length != 2)
        {
            logger.InvalidOffsetFormat(commitedOffsetString);

            return new()
            {
                Position = null,
                LastReadLine = commitedOffsetString,
            };
        }

        return new()
        {
            Position = long.TryParse(positionAndLineStrings[0], out long parsedPosition) ? parsedPosition : null,
            LastReadLine = positionAndLineStrings[1],
        };
    }

    private static async Task CommitOffset(
        IOffsetService offsetService,
        string infobaseFullPath,
        long? position,
        string lastReadLine,
        CancellationToken cancellationToken)
    {
        await offsetService.CommitOffset(
            key: infobaseFullPath,
            offset: position != null ? $"{position}{OffsetValuesSeparator}{lastReadLine}" : lastReadLine,
            cancellationToken);
    }

    private static List<ObjectFilter> GetObjectFilters(V77ApplicationProducerSettings settings)
    {
        return settings.ObjectFilters
            .Select(f => f.Split(ObjectFilterValuesSeparator))
            .Select(splitted => new ObjectFilter()
            {
                Id = splitted[0],
                Depth = int.TryParse(splitted[1], out int depth) ? depth : ObjectFilterDefaultDepth
            })
            .ToList();
    }

    [GeneratedRegex(@"\s+(\d{2}\.\d{2}\.\d{4})\s?")]
    private static partial Regex ObjectDateRegex();

    public class FailedToGetObjectException : Exception
    {
        internal FailedToGetObjectException(string objectId)
            : base($"Failed to get object with id '{objectId}'")
        {
        }
    }
}
