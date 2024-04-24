using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using static KrasnyyOktyabr.Application.Logging.KafkaLoggingHelper;
using static KrasnyyOktyabr.Application.Services.Kafka.V77ApplicationProducerService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

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

    public int ManagedInstancesCount => _producers?.Count ?? 0;

    public List<V83ApplicationProducerStatus> Status
    {
        get
        {
            if (_producers == null || _producers.Count == 0)
            {
                return [];
            }

            List<V83ApplicationProducerStatus> statuses = new(_producers.Count);

            foreach (V83ApplicationProducer producer in _producers.Values)
            {
                statuses.Add(new()
                {
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

            return statuses;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Starting ...");

        StartProducers();

        return Task.CompletedTask;
    }

    public async ValueTask RestartAsync(CancellationToken cancellationToken)
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
            logger.ConfigurationNotFound();

            _producers = null;

            return;
        }

        logger.ConfigurationFound(producersSettings.Length);

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
                logger.StoppingProducers(_producers.Count);
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

        private readonly V83ApplicationProducerSettings _settings;

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
            _settings = settings;
            _offsetService = offsetService;
            _httpClientFactory = httpClientFactory;

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _getLogTransactionsTask = getLogTransactionsTask;
            _producerTask = Task.Run(() => RunProducerAsync(cancellationToken), cancellationToken);

            LastActivity = DateTimeOffset.Now;
        }

        public string Key => _settings.InfobaseUrl;

        public bool Active => Error == null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public string InfobaseUrl => _settings.InfobaseUrl;

        public string Username => _settings.Username;

        public int Fetched { get; private set; }

        public int Produced { get; private set; }

        public string DataTypeJsonPropertyName => _settings.DataTypePropertyName;

        public IReadOnlyList<string> ObjectFilters => _settings.ObjectFilters.AsReadOnly();

        public IReadOnlyList<string> TransactionTypes => _settings.TransactionTypeFilters;

        public Exception? Error { get; private set; }

        private async Task RunProducerAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    LastActivity = DateTimeOffset.Now;

                    _logger.RequestInfobaseChanges(_settings.InfobaseUrl);

                    string changes = await _getLogTransactionsTask(
                        _settings,
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
                _logger.OperationCancelled();
            }
            catch (Exception ex)
            {
                Error = ex;

                _logger.ErrorOnInfobaseChange(ex);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _logger.Disposing(Key);

            _cancellationTokenSource.Cancel();

            await _producerTask.ConfigureAwait(false);
        }
    }
}
