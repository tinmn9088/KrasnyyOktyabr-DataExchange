#nullable enable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Logging.KafkaLoggingHelper;
using static KrasnyyOktyabr.ApplicationNet48.Services.Kafka.V77ApplicationProducerService;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public sealed class V83ApplicationProducerService(
    IConfiguration configuration,
    IOffsetService offsetService,
    IHttpClientFactory httpClientFactory,
    ILogger<V83ApplicationProducerService> logger,
    ILoggerFactory loggerFactory)
    : IV83ApplicationProducerService
{
    public delegate ValueTask<string> GetLogTransactionsAsync(
        V83ApplicationProducerSettings settings,
        IOffsetService offsetService,
        IHttpClientFactory httpClientFactory,
        ILogger logger,
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

    public int ManagedInstancesCount => _producers?.Count ?? 0;

    public IStatusContainer<V83ApplicationProducerStatus> Status
    {
        get
        {
            if (_producers == null || _producers.Count == 0)
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
                    ObjectFilters = producer.ObjectFilters,
                    TransactionTypeFilters = producer.TransactionTypes,
                    Fetched = producer.Fetched,
                    Produced = producer.Produced,
                    InfobaseUrl = producer.InfobaseUrl,
                    Username = producer.Username,
                    DataTypePropertyName = producer.DataTypeJsonPropertyName,
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
            if (_producers != null && _producers.TryGetValue(key, out V83ApplicationProducer? producer))
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

    public GetLogTransactionsAsync GetLogTransactionsTask => (
        V83ApplicationProducerSettings settings,
        IOffsetService offsetService,
        IHttpClientFactory httpClientFactory,
        ILogger logger,
        CancellationToken cancellationToken) =>
    {
        throw new NotImplementedException(); // TODO: get log transactions
    };

    private void StartProducers()
    {
        V83ApplicationProducerSettings[]? producersSettings = GetProducersSettings();

        if (producersSettings == null)
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
        => ValidationHelper.GetAndValidateKafkaClientSettings<V83ApplicationProducerSettings>(configuration, V83ApplicationProducerSettings.Position, logger);

    /// <summary>
    /// Creates new <see cref="V83ApplicationProducer"/> and saves it to <see cref="_producers"/>.
    /// </summary>
    private void StartProducer(V83ApplicationProducerSettings settings)
    {
        _producers ??= [];

        V83ApplicationProducer producer = new(
            loggerFactory.CreateLogger<V83ApplicationProducer>(),
            settings,
            offsetService,
            httpClientFactory,
            GetLogTransactionsTask);

        _producers.Add(producer.Key, producer);
    }

    private async Task StopProducersAsync()
    {
        if (_producers != null)
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

        private readonly IOffsetService _offsetService;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly GetLogTransactionsAsync _getLogTransactionsTask;

        private readonly Task _producerTask;

        /// <remarks>
        /// Need to be disposed.
        /// </remarks>
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal V83ApplicationProducer(
            ILogger<V83ApplicationProducer> logger,
            V83ApplicationProducerSettings settings,
            IOffsetService offsetService,
            IHttpClientFactory httpClientFactory,
            GetLogTransactionsAsync getLogTransactionsTask)
        {
            _logger = logger;
            Settings = settings;
            _offsetService = offsetService;
            _httpClientFactory = httpClientFactory;

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _getLogTransactionsTask = getLogTransactionsTask;
            _producerTask = Task.Run(() => RunProducerAsync(cancellationToken), cancellationToken);

            LastActivity = DateTimeOffset.Now;
        }

        public V83ApplicationProducerSettings Settings { get; private set; }

        public string Key => Settings.InfobaseUrl;

        public bool Active => Error == null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public string InfobaseUrl => Settings.InfobaseUrl;

        public string Username => Settings.Username;

        public int Fetched { get; private set; }

        public int Produced { get; private set; }

        public string DataTypeJsonPropertyName => Settings.DataTypePropertyName;

        public IReadOnlyList<string> ObjectFilters => Settings.ObjectFilters;

        public IReadOnlyList<string> TransactionTypes => Settings.TransactionTypeFilters;

        public Exception? Error { get; private set; }

        private async Task RunProducerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    LastActivity = DateTimeOffset.Now;

                    _logger.LogRequestInfobaseChanges(Key);

                    string changes = await _getLogTransactionsTask(
                        Settings,
                        _offsetService,
                        _httpClientFactory,
                        _logger,
                        cancellationToken);

                    LastActivity = DateTimeOffset.Now;

                    // TODO: process changes

                    await Task.Delay(RequestInterval, cancellationToken).ConfigureAwait(false);
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
}
