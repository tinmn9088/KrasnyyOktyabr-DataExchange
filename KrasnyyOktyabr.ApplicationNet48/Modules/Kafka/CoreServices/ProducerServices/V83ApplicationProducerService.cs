#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Common.Helpers;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Common.Logging.KafkaLoggingHelper;
using static KrasnyyOktyabr.ApplicationNet48.Common.Helpers.TimeHelper;
using static KrasnyyOktyabr.ApplicationNet48.Common.Helpers.HttpClientHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.CoreServices.ProducerServices;

public sealed class V83ApplicationProducerService(
    IConfiguration configuration,
    IJsonService jsonService,
    IOffsetService offsetService,
    IHttpClientFactory httpClientFactory,
    IKafkaService kafkaService,
    ILogger<V83ApplicationProducerService> logger,
    ILoggerFactory loggerFactory)
    : IV83ApplicationProducerService
{
    public readonly struct NewLogTransactionResponse(string nextDate, string? transaction, string? previousTransaction, string data, string? dataType)
    {
        public string NextDate { get; } = nextDate;

        public string? Transaction { get; } = transaction;

        public string? PreviousTransaction { get; } = previousTransaction;

        public string Data { get; } = data;

        public string? DataType { get; } = dataType;
    }

    public static string LogTransactionNextDateJsonPropertyName => "nextdate";

    public static string LogTransactionTransactionJsonPropertyName => "transaction";

    public static string LogTransactionDataTypeJsonPropertyName => "datatype";

    public static string LogTransactionDataJsonPropertyName => "data";

    /// <returns>New transaction JSON.</returns>
    public delegate ValueTask<NewLogTransactionResponse> GetNewLogTransactionAsync(
        V83ApplicationProducerSettings settings,
        IJsonService jsonService,
        IOffsetService offsetService,
        IHttpClientFactory httpClientFactory,
        ILogger logger,
        CancellationToken cancellationToken);

    public delegate ValueTask SendObjectJsonAsync(
        V83ApplicationProducerSettings settings,
        NewLogTransactionResponse logTransaction,
        string messageKey,
        IKafkaService kafkaService,
        CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    /// Is <c>null</c> when no configuration found.
    /// </para>
    /// <para>
    /// Keys are results of <see cref="V77ApplicationProducer.Key"/>.
    /// </para>
    /// </summary>
    private Dictionary<string, V83ApplicationProducer>? _producers;

    /// <summary>
    /// Synchronizes restart methods.
    /// </summary>
    private readonly SemaphoreSlim _restartLock = new(1, 1);

    public static readonly Regex InfobasePubNameRegex = new(@"/([^/]+)");

    public int ManagedInstancesCount => _producers?.Count ?? 0;

    public static string NoNewTransactionsResponseContent => "null";

    public static string V83ApplicationDateFormat => "yyyyMMdd";

    private const char OffsetValuesSeparator = '&';

    public IStatusContainer<V83ApplicationProducerStatus> Status
    {
        get
        {
            if (_producers is null || _producers.Count == 0)
            {
                return StatusContainer<V83ApplicationProducerStatus>.Empty;
            }

            List<V83ApplicationProducerStatus> statuses = new(_producers.Count);

            foreach (V83ApplicationProducer producer in _producers.Values)
            {
                statuses.Add(new()
                {
                    ServiceKey = producer.Key,
                    Active = producer.Active,
                    LastActivity = producer.LastActivity,
                    ErrorMessage = producer.Error?.Message,
                    ObjectFilters = producer.CacheObjectFiltersList,
                    TransactionTypeFilters = producer.Settings.TransactionTypeFilters,
                    Fetched = producer.Fetched,
                    Produced = producer.Produced,
                    InfobaseUrl = producer.InfobaseUrl,
                    Username = producer.Username,
                    DataTypePropertyName = producer.DataTypeJsonPropertyName,
                    SuspendSchedule = producer.Settings.SuspendSchedule,
                });
            }

            return new StatusContainer<V83ApplicationProducerStatus>()
            {
                Statuses = statuses,
            };
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogStarting();

        try
        {
            StartProducers();

            logger.LogStarted();
        }
        catch (Exception ex)
        {
            logger.LogErrorOnStart(ex);
        }

        return Task.CompletedTask;
    }

    public async ValueTask RestartAsync(CancellationToken cancellationToken)
    {
        logger.LogRestarting();

        await _restartLock.WaitAsync(cancellationToken);

        try
        {
            await StopProducersAsync();

            StartProducers();

            logger.LogRestarted();
        }
        finally
        {
            _restartLock.Release();
        }
    }

    public async ValueTask RestartAsync(string key, CancellationToken cancellationToken)
    {
        logger.LogRestarting(key);

        await _restartLock.WaitAsync(cancellationToken);

        try
        {
            if (_producers is not null && _producers.TryGetValue(key, out V83ApplicationProducer? producer))
            {
                _producers.Remove(key);

                StartProducer(producer.Settings);
            }
        }
        finally
        {
            _restartLock.Release();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogStopping();

        await StopProducersAsync();

        logger.LogStopped();
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogDisposing();

        await StopProducersAsync();

        logger.LogDisposed();
    }

    public GetNewLogTransactionAsync GetNewLogTransactionTask => async (
        V83ApplicationProducerSettings settings,
        IJsonService jsonService,
        IOffsetService offsetService,
        IHttpClientFactory httpClientFactory,
        ILogger logger,
        CancellationToken cancellationToken) =>
    {
        LogOffset offset = await GetCommitedOffset(offsetService, settings.InfobaseUrl, logger, cancellationToken);

        using HttpClient httpClient = httpClientFactory.CreateClient();

        HttpRequestMessage request = new(HttpMethod.Post, settings.InfobaseUrl)
        {
            Content = new StringContent(jsonService.Serialize(new V83ApplicationProducerNewTransactionRequest
            {
                ObjectFilters = settings.ObjectFilters
                    .Select(f => new V83ApplicationProducerNewTransactionRequestObjectFilter
                    {
                        DataType = f.DataType,
                        JsonDepth = f.JsonDepth,
                    })
                    .ToArray(),
                TransactionTypeFilters = settings.TransactionTypeFilters,
                TransactionToStartAfter = offset.Transaction,
                StartDate = offset.Date,
            }))
        };

        if (settings.Username is not null)
        {
            request.Headers.Authorization = GetAuthenticationHeaderValue(settings.Username, settings.Password);
        }

        HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

        // Check response
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            throw new FailedToGetNewLogTransactionException(settings.InfobaseUrl, (int)response.StatusCode, responseContent);
        }

        string newTransactionJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        // Extract transaction data
        Dictionary<string, string?> extractedValues = jsonService.ExtractProperties(
            newTransactionJson,
            [
                LogTransactionNextDateJsonPropertyName,
                LogTransactionTransactionJsonPropertyName,
                LogTransactionDataTypeJsonPropertyName,
                LogTransactionDataJsonPropertyName
            ]);

        string nextDate = extractedValues[LogTransactionNextDateJsonPropertyName]
            ?? throw new MissingLogTransactionPropertyException(LogTransactionNextDateJsonPropertyName);
        string? transaction = extractedValues[LogTransactionTransactionJsonPropertyName];
        string? dataType = extractedValues[LogTransactionDataTypeJsonPropertyName];
        string? data = extractedValues[LogTransactionDataJsonPropertyName];

        return new NewLogTransactionResponse(nextDate, transaction, offset.Transaction, newTransactionJson, dataType);
    };

    public SendObjectJsonAsync SendObjectJsonTask = async (
        V83ApplicationProducerSettings settings,
        NewLogTransactionResponse logTransaction,
        string infobasePubName,
        IKafkaService kafkaService,
        CancellationToken cancellationToken) =>
    {
        // 1. Determine Kafka topic name

        // 1.1 Determine if Kafka topic name was specified in settings
        string? topicFromSettings = settings.ObjectFilters
            .Where(f => logTransaction.DataType!.StartsWith(f.DataType))
            .Select(f => f.Topic)
            .FirstOrDefault();

        // 1.2 Use Kafka topic name from settings or generate
        string topicName = topicFromSettings is not null
            ? topicFromSettings
            : kafkaService.BuildTopicName(infobasePubName, logTransaction.DataType);

        // 2. Prepare Kafka message
        Message<string, string> kafkaMessage = new()
        {
            Key = infobasePubName,
            Value = logTransaction.Data,
        };

        using IProducer<string, string> producer = kafkaService.GetProducer<string, string>();

        await producer.ProduceAsync(topicName, kafkaMessage, cancellationToken).ConfigureAwait(false);

        logger.LogProducedMessage(topicName, kafkaMessage.Key, kafkaMessage.Value);
    };

    private void StartProducers()
    {
        V83ApplicationProducerSettings[]? producersSettings = GetProducersSettings();

        if (producersSettings is null)
        {
            logger.LogConfigurationNotFound();

            _producers = null;

            return;
        }

        logger.LogConfigurationFound(producersSettings.Length);

        foreach (V83ApplicationProducerSettings settings in producersSettings)
        {
            StartProducer(settings);
        }
    }

    private V83ApplicationProducerSettings[]? GetProducersSettings()
        => ValidationHelper.GetAndValidateVApplicationKafkaProducerSettings<V83ApplicationProducerSettings>(configuration, V83ApplicationProducerSettings.Position, logger);

    /// <summary>
    /// Creates new <see cref="V83ApplicationProducer"/> and saves it to <see cref="_producers"/>.
    /// </summary>
    private void StartProducer(V83ApplicationProducerSettings settings)
    {
        _producers ??= [];

        V83ApplicationProducer producer = new(
            loggerFactory.CreateLogger<V83ApplicationProducer>(),
            settings,
            jsonService,
            offsetService,
            kafkaService,
            httpClientFactory,
            GetNewLogTransactionTask,
            SendObjectJsonTask);

        _producers.Add(producer.Key, producer);
    }

    private async Task StopProducersAsync()
    {
        if (_producers is not null)
        {
            if (_producers.Count > 0)
            {
                logger.LogStoppingProducers(_producers.Count);
            }

            foreach (V83ApplicationProducer producer in _producers.Values)
            {
                await producer.DisposeAsync();
            }

            _producers.Clear();
        }
    }

    private sealed class V83ApplicationProducer : IAsyncDisposable
    {
        private static TimeSpan RequestInterval => TimeSpan.FromSeconds(3);

        private readonly ILogger<V83ApplicationProducer> _logger;

        private readonly string _infobasePubName;

        private readonly IJsonService _jsonService;

        private readonly IOffsetService _offsetService;

        private readonly IKafkaService _kafkaService;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly GetNewLogTransactionAsync _getNewLogTransactionTask;

        private readonly SendObjectJsonAsync _sendObjectJsonTask;

        private readonly Task _producerTask;

        /// <remarks>
        /// Need to be disposed.
        /// </remarks>
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal V83ApplicationProducer(
            ILogger<V83ApplicationProducer> logger,
            V83ApplicationProducerSettings settings,
            IJsonService jsonService,
            IOffsetService offsetService,
            IKafkaService kafkaService,
            IHttpClientFactory httpClientFactory,
            GetNewLogTransactionAsync getLogTransactionsTask,
            SendObjectJsonAsync sendObjectJsonTask)
        {
            _logger = logger;
            Settings = settings;
            _jsonService = jsonService;
            _offsetService = offsetService;
            _kafkaService = kafkaService;
            _httpClientFactory = httpClientFactory;

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _getNewLogTransactionTask = getLogTransactionsTask;
            _sendObjectJsonTask = sendObjectJsonTask;
            _producerTask = Task.Run(() => RunProducerAsync(cancellationToken), cancellationToken);

            // Prepare cached values
            CacheObjectFiltersList = Settings.ObjectFilters.Select(f => new ObjectFilter(f.DataType, f.JsonDepth, f.Topic)).ToList().AsReadOnly();

            // Extract infobase publication name
            MatchCollection matches = InfobasePubNameRegex.Matches(settings.InfobaseUrl);
            _infobasePubName = matches[1].Groups[1].Value;

            if (_infobasePubName.Length == 0)
            {
                throw new ArgumentException($"Empty infobase pub name: {settings.InfobaseUrl}");
            }

            LastActivity = DateTimeOffset.Now;
        }

        public V83ApplicationProducerSettings Settings { get; private set; }

        public string Key => Settings.InfobaseUrl;

        public bool Active => Error is null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public string InfobaseUrl => Settings.InfobaseUrl;

        public string Username => Settings.Username;

        public int Fetched { get; private set; }

        public int Produced { get; private set; }

        public string DataTypeJsonPropertyName => Settings.DataTypePropertyName;

        public IReadOnlyList<ObjectFilter> CacheObjectFiltersList { get; private set; }

        public Exception? Error { get; private set; }

        private async Task RunProducerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    LastActivity = DateTimeOffset.Now;

                    if (Settings.SuspendSchedule is not null)
                    {
                        await WaitPeriodsEndAsync(() => DateTimeOffset.Now, Settings.SuspendSchedule, cancellationToken, _logger);
                    }

                    _logger.LogRequestNewLogTransactions(Key);

                    NewLogTransactionResponse newLogTransaction = await _getNewLogTransactionTask(
                        Settings,
                        _jsonService,
                        _offsetService,
                        _httpClientFactory,
                        _logger,
                        cancellationToken)
                        .ConfigureAwait(false);

                    LastActivity = DateTimeOffset.Now;

                    bool isLogTransactionPresent = newLogTransaction.Transaction is not null
                        && newLogTransaction.Transaction is not null
                        && newLogTransaction.DataType is not null;

                    if (isLogTransactionPresent)
                    {
                        await _sendObjectJsonTask(
                            Settings,
                            newLogTransaction,
                            _infobasePubName,
                            _kafkaService,
                            cancellationToken)
                            .ConfigureAwait(false);
                    }

                    await CommitOffset(
                        _offsetService,
                        infobaseUrl: Settings.InfobaseUrl,
                        nextDate: newLogTransaction.NextDate,
                        transaction: newLogTransaction.Transaction is not null ? newLogTransaction.Transaction : newLogTransaction.PreviousTransaction,
                        cancellationToken)
                        .ConfigureAwait(false);

                    if (!isLogTransactionPresent)
                    {
                        await Task.Delay(RequestInterval, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogOperationCancelled();
            }
            catch (Exception ex)
            {
                Error = ex;

                _logger.LogProducerError(ex);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogDisposing(Key);

            try
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();

                await _producerTask.ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on dispose");
            }

            _logger.LogDisposed(Key);
        }
    }

    internal readonly struct LogOffset(string date, string? transaction)
    {
        public string Date { get; } = date;

        public string? Transaction { get; } = transaction;
    }

    private static async Task<LogOffset> GetCommitedOffset(IOffsetService offsetService, string infobaseUrl, ILogger logger, CancellationToken cancellationToken)
    {
        string? commitedOffsetString = await offsetService.GetOffset(infobaseUrl, cancellationToken).ConfigureAwait(false);

        if (commitedOffsetString is null)
        {
            return new(
                date: DateTime.Now.ToString(V83ApplicationDateFormat),
                transaction: null
            );
        }

        string[] dateAndTransactionStrings = commitedOffsetString.Split(OffsetValuesSeparator);

        if (dateAndTransactionStrings.Length < 2)
        {
            logger.LogOffsetInvalidFormat(commitedOffsetString);

            return new(
                date: DateTime.Now.ToString(V83ApplicationDateFormat),
                transaction: commitedOffsetString
            );
        }

        return new(
            date: dateAndTransactionStrings[0],
            transaction: dateAndTransactionStrings[1]
        );
    }

    private static async Task CommitOffset(
        IOffsetService offsetService,
        string infobaseUrl,
        string nextDate,
        string? transaction,
        CancellationToken cancellationToken)
    {
        await offsetService.CommitOffset(
            key: infobaseUrl,
            offset: $"{nextDate}{OffsetValuesSeparator}{transaction}",
            cancellationToken);
    }

    public class FailedToGetNewLogTransactionException : Exception
    {
        internal FailedToGetNewLogTransactionException(string infobaseUrl, int statusCode, string responseContent)
            : base($"Failed to get new log transaction from '{infobaseUrl}' (code - {statusCode}): {responseContent}")
        {
        }
    }

    public class MissingLogTransactionPropertyException : Exception
    {
        internal MissingLogTransactionPropertyException(string propertyName)
            : base($"Missing transaction property '{propertyName}'")
        {
        }
    }
}
