using System.Runtime.Versioning;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using static KrasnyyOktyabr.Application.Logging.KafkaLoggingHelper;

namespace KrasnyyOktyabr.Application.Services.Kafka;

[SupportedOSPlatform("windows")]
public sealed class V77ApplicationConsumerService(
    IConfiguration configuration,
    IJsonService jsonService,
    IComV77ApplicationConnectionFactory connectionFactory,
    IKafkaService kafkaService,
    ITransliterationService transliterationService,
    ILogger<V77ApplicationConsumerService> logger,
    ILoggerFactory loggerFactory)
    : IV77ApplicationConsumerService
{
    public delegate ValueTask<List<string>?> TransformMessageAsync(
        string topic,
        string message,
        V77ApplicationConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken);

    public delegate ValueTask V77ApplicationSaveAsync(
        List<string> jsonTransformResults,
        V77ApplicationConsumerSettings settings,
        IComV77ApplicationConnectionFactory connectionFactory,
        CancellationToken cancellationToken);

    public static string DefaultErtRelativePath => @"ExtForms\EDO\Test\SaveObject.ert";

    /// <summary>
    /// <para>
    /// Is <c>null</c> when no configuration found.
    /// </para>
    /// <para>
    /// Keys are results of <see cref="V77ApplicationProducer.Key"/>.
    /// </para>
    /// </summary>
    private Dictionary<string, V77ApplicationConsumer>? _consumers;

    /// <summary>
    /// Synchronizes restart methods.
    /// </summary>
    private readonly SemaphoreSlim _restartLock = new(1, 1);

    public int ManagedInstancesCount => _consumers?.Count ?? 0;

    public IStatusContainer<V77ApplicationConsumerStatus> Status
    {
        get
        {
            if (_consumers == null || _consumers.Count == 0)
            {
                return StatusContainer<V77ApplicationConsumerStatus>.Empty;
            }

            List<V77ApplicationConsumerStatus> statuses = new(_consumers.Count);

            foreach (V77ApplicationConsumer consumer in _consumers.Values)
            {
                statuses.Add(new()
                {
                    ServiceKey = consumer.Key,
                    Active = consumer.Active,
                    LastActivity = consumer.LastActivity,
                    ErrorMessage = consumer.Error?.Message,
                    InfobaseName = consumer.InfobaseName,
                    Consumed = consumer.Consumed,
                    Saved = consumer.Saved,
                    Topics = consumer.Topics,
                    ConsumerGroup = consumer.ConsumerGroup,
                });
            }

            return new StatusContainer<V77ApplicationConsumerStatus>()
            {
                Statuses = statuses,
            };
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Starting ...");

        StartConsumers();

        return Task.CompletedTask;
    }

    public async ValueTask RestartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Restarting ...");

        await _restartLock.WaitAsync(cancellationToken);

        try
        {
            await StopConsumersAsync();

            StartConsumers();

            logger.LogTrace("Restarted");
        }
        finally
        {
            _restartLock.Release();
        }
    }

    public async ValueTask RestartAsync(string key, CancellationToken cancellationToken)
    {
        logger.RestartingByKey(key);

        await _restartLock.WaitAsync(cancellationToken);

        try
        {
            if (_consumers != null && _consumers.TryGetValue(key, out V77ApplicationConsumer? consumer))
            {
                _consumers.Remove(key);

                StartConsumer(consumer.Settings);

                logger.RestartedWithKey(key);
            }
        }
        finally
        {
            _restartLock.Release();
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Stopping ...");

        await StopConsumersAsync();
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogTrace("Disposing ...");

        await StopConsumersAsync();
    }

    public TransformMessageAsync TransformMessageTask => async (
        string topic,
        string message,
        V77ApplicationConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken) =>
    {
        List<string> jsonTransformResults;

        try
        {
            if (!settings.TopicsInstructionNames.TryGetValue(topic, out string? instructionName))
            {
                throw new InstructionNotSpecifiedException(topic);
            }

            jsonTransformResults = await jsonService.RunJsonTransformOnConsumedMessageVApplicationAsync(
                instructionName,
                message,
                cancellationToken);

            logger.JsonTransformResult(jsonTransformResults.Count);
        }
        catch (Exception ex)
        {
            logger.JsonTransformError(ex);

            return null;
        }

        return jsonTransformResults;
    };

    public V77ApplicationSaveAsync V77ApplicationSaveTask => async (
        List<string> jsonTransformResults,
        V77ApplicationConsumerSettings settings,
        IComV77ApplicationConnectionFactory connectionFactory,
        CancellationToken cancellationToken) =>
    {
        string infobaseFullPath = GetInfobaseFullPath(settings.InfobasePath);

        logger.SavingObjects(jsonTransformResults.Count);

        ConnectionProperties connectionProperties = new()
        {
            InfobasePath = infobaseFullPath,
            Username = settings.Username,
            Password = settings.Password,
        };

        await using (IComV77ApplicationConnection connection = await connectionFactory.GetConnectionAsync(connectionProperties, cancellationToken).ConfigureAwait(false))
        {
            await connection.ConnectAsync(cancellationToken).ConfigureAwait(false);

            foreach (string result in jsonTransformResults)
            {
                Dictionary<string, object?> ertContext = new()
                {
                    { "ObjectJson", result },
                };

                await connection.RunErtAsync(
                    ertRelativePath: GetErtRelativePath(settings),
                    ertContext,
                    resultName: null,
                    cancellationToken).ConfigureAwait(false);
            }
        }
    };

    private void StartConsumers()
    {
        V77ApplicationConsumerSettings[]? producersSettings = GetConsumersSettings();

        if (producersSettings == null)
        {
            logger.ConfigurationNotFound();

            _consumers = null;

            return;
        }

        logger.ConfigurationFound(producersSettings.Length);

        foreach (V77ApplicationConsumerSettings settings in producersSettings)
        {
            StartConsumer(settings);
        }
    }

    private V77ApplicationConsumerSettings[]? GetConsumersSettings()
        => ValidationHelper.GetAndValidateKafkaClientSettings<V77ApplicationConsumerSettings>(configuration, V77ApplicationConsumerSettings.Position, logger);

    /// <summary>
    /// Creates new <see cref="V77ApplicationConsumer"/> and saves it to <see cref="_consumers"/>.
    /// </summary>
    private void StartConsumer(V77ApplicationConsumerSettings settings)
    {
        _consumers ??= [];
        V77ApplicationConsumer consumer = new(
            loggerFactory.CreateLogger<V77ApplicationConsumer>(),
            settings,
            kafkaService,
            jsonService,
            connectionFactory,
            transliterationService,
            TransformMessageTask,
            V77ApplicationSaveTask);

        _consumers.Add(consumer.Key, consumer);
    }

    private async Task StopConsumersAsync()
    {
        if (_consumers != null)
        {
            if (_consumers.Count > 0)
            {
                logger.StoppingConsumers(_consumers.Count);
            }

            foreach (V77ApplicationConsumer consumer in _consumers.Values)
            {
                await consumer.DisposeAsync();
            }

            _consumers.Clear();
        }
    }

    private sealed class V77ApplicationConsumer : IAsyncDisposable
    {
        private readonly ILogger<V77ApplicationConsumer> _logger;

        private readonly V77ApplicationConsumerSettings _settings;

        private readonly IKafkaService _kafkaService;

        private readonly IJsonService _jsonService;

        private readonly IComV77ApplicationConnectionFactory _connectionFactory;

        private readonly TransformMessageAsync _transformMessageTask;

        private readonly V77ApplicationSaveAsync _v77ApplicationSaveTask;

        private readonly Task _consumerTask;

        /// <remarks>
        /// Need to be disposed.
        /// </remarks>
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal V77ApplicationConsumer(
            ILogger<V77ApplicationConsumer> logger,
            V77ApplicationConsumerSettings settings,
            IKafkaService kafkaService,
            IJsonService jsonService,
            IComV77ApplicationConnectionFactory connectionFactory,
            ITransliterationService transliterationService,
            TransformMessageAsync transformMessageTask,
            V77ApplicationSaveAsync v77ApplicationSaveTask)
        {
            _logger = logger;
            _settings = settings;
            _kafkaService = kafkaService;
            _jsonService = jsonService;
            _connectionFactory = connectionFactory;

            InfobaseName = ExtractInfobaseName(settings.InfobasePath);

            if (settings.ConsumerGroup != null)
            {
                ConsumerGroup = settings.ConsumerGroup;
            }
            else
            {
                logger.ConsumerGroupNotPresent();

                ConsumerGroup = transliterationService.TransliterateToLatin(InfobaseName);
            }

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _transformMessageTask = transformMessageTask;
            _v77ApplicationSaveTask = v77ApplicationSaveTask;
            _consumerTask = Task.Run(() => RunConsumerAsync(cancellationToken), cancellationToken);

            LastActivity = DateTimeOffset.Now;
        }

        public V77ApplicationConsumerSettings Settings => _settings;

        public string Key => _settings.InfobasePath;

        public bool Active => Error == null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public IReadOnlyList<string> Topics => _settings.Topics;

        public string InfobaseName { get; init; }

        public string ConsumerGroup { get; private set; }

        public int Consumed { get; private set; }

        public int Saved { get; private set; }

        public Exception? Error { get; private set; }

        private async Task RunConsumerAsync(CancellationToken cancellationToken)
        {
            try
            {
                IConsumer<string, string> consumer = _kafkaService.GetConsumer<string, string>(_settings.Topics, ConsumerGroup);

                while (!cancellationToken.IsCancellationRequested)
                {
                    LastActivity = DateTimeOffset.Now;

                    ConsumeResult<string, string> consumeResult = consumer.Consume(cancellationToken);

                    _logger.ConsumingMessage(
                        ConsumerGroup,
                        topicName: consumeResult.Topic,
                        key: consumeResult.Message.Key,
                        length: consumeResult.Message.Value.Length,
                        shortenedMessage: ShortenMessage(consumeResult.Message.Value, 400));

                    LastActivity = DateTimeOffset.Now;

                    List<string>? jsonTransformResults = await _transformMessageTask(
                        topic: consumeResult.Topic,
                        message: consumeResult.Message.Value,
                        _settings,
                        _jsonService,
                        _logger,
                        cancellationToken);

                    if (jsonTransformResults == null || jsonTransformResults.Count == 0)
                    {
                        continue;
                    }

                    await _v77ApplicationSaveTask(
                        jsonTransformResults,
                        _settings,
                        _connectionFactory,
                        cancellationToken);
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

            await _consumerTask.ConfigureAwait(false);
        }

        private static string ExtractInfobaseName(string infobasePath) => Path.GetFileName(infobasePath);
    }

    private static string GetInfobaseFullPath(string infobasePath) => Path.GetFullPath(infobasePath);

    private static string GetErtRelativePath(V77ApplicationConsumerSettings settings) => settings.ErtRelativePath ?? DefaultErtRelativePath;

    public class InstructionNotSpecifiedException : Exception
    {
        internal InstructionNotSpecifiedException(string topic) : base($"Instruction not specified for '{topic}'")
        {
        }
    }
}
