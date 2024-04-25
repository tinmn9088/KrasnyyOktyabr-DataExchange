using System.Text.RegularExpressions;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using static KrasnyyOktyabr.Application.Logging.KafkaLoggingHelper;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public sealed partial class V83ApplicationConsumerService(
    IConfiguration configuration,
    IJsonService jsonService,
    IHttpClientFactory httpClientFactory,
    IKafkaService kafkaService,
    ITransliterationService transliterationService,
    ILogger<V83ApplicationConsumerService> logger,
    ILoggerFactory loggerFactory)
    : IV83ApplicationConsumerService
{
    public delegate ValueTask<List<string>?> TransformMessageAsync(
        string topic,
        string message,
        V83ApplicationConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken);

    public delegate ValueTask V83ApplicationSaveAsync(
        List<string> jsonTransformResults,
        V83ApplicationConsumerSettings settings,
        IHttpClientFactory httpClientFactory,
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
    private Dictionary<string, V83ApplicationConsumer>? _consumers;

    /// <summary>
    /// Synchronizes restart methods.
    /// </summary>
    private readonly SemaphoreSlim _restartLock = new(1, 1);

    public int ManagedInstancesCount => _consumers?.Count ?? 0;

    public IStatusContainer<V83ApplicationConsumerStatus> Status
    {
        get
        {
            if (_consumers == null || _consumers.Count == 0)
            {
                return StatusContainer<V83ApplicationConsumerStatus>.Empty;
            }

            List<V83ApplicationConsumerStatus> statuses = new(_consumers.Count);

            foreach (V83ApplicationConsumer consumer in _consumers.Values)
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

            return new StatusContainer<V83ApplicationConsumerStatus>()
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
            if (_consumers != null && _consumers.TryGetValue(key, out V83ApplicationConsumer? consumer))
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
        V83ApplicationConsumerSettings settings,
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

    public V83ApplicationSaveAsync V83ApplicationSaveTask => (
        List<string> jsonTransformResults,
        V83ApplicationConsumerSettings settings,
        IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken) =>
    {
        throw new NotImplementedException(); // TODO: send objects to infobase
    };

    private void StartConsumers()
    {
        V83ApplicationConsumerSettings[]? producersSettings = GetConsumersSettings();

        if (producersSettings == null)
        {
            logger.ConfigurationNotFound();

            _consumers = null;

            return;
        }

        logger.ConfigurationFound(producersSettings.Length);

        foreach (V83ApplicationConsumerSettings settings in producersSettings)
        {
            StartConsumer(settings);
        }
    }

    private V83ApplicationConsumerSettings[]? GetConsumersSettings()
        => ValidationHelper.GetAndValidateKafkaClientSettings<V83ApplicationConsumerSettings>(configuration, V83ApplicationConsumerSettings.Position, logger);

    /// <summary>
    /// Creates new <see cref="V83ApplicationConsumer"/> and saves it to <see cref="_consumers"/>.
    /// </summary>
    private void StartConsumer(V83ApplicationConsumerSettings settings)
    {
        _consumers ??= [];
        V83ApplicationConsumer consumer = new(
            loggerFactory.CreateLogger<V83ApplicationConsumer>(),
            settings,
            kafkaService,
            jsonService,
            httpClientFactory,
            transliterationService,
            TransformMessageTask,
            V83ApplicationSaveTask);

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

            foreach (V83ApplicationConsumer consumer in _consumers.Values)
            {
                await consumer.DisposeAsync();
            }

            _consumers.Clear();
        }
    }

    private sealed partial class V83ApplicationConsumer : IAsyncDisposable
    {
        private readonly ILogger<V83ApplicationConsumer> _logger;

        private readonly IKafkaService _kafkaService;

        private readonly IJsonService _jsonService;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly TransformMessageAsync _transformMessageTask;

        private readonly V83ApplicationSaveAsync _v77ApplicationSaveTask;

        private readonly Task _consumerTask;

        /// <remarks>
        /// Need to be disposed.
        /// </remarks>
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal V83ApplicationConsumer(
            ILogger<V83ApplicationConsumer> logger,
            V83ApplicationConsumerSettings settings,
            IKafkaService kafkaService,
            IJsonService jsonService,
            IHttpClientFactory httpClientFactory,
            ITransliterationService transliterationService,
            TransformMessageAsync transformMessageTask,
            V83ApplicationSaveAsync v77ApplicationSaveTask)
        {
            _logger = logger;
            Settings = settings;
            _kafkaService = kafkaService;
            _jsonService = jsonService;
            _httpClientFactory = httpClientFactory;

            InfobaseName = ExtractInfobaseName(settings.InfobaseUrl);

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

        public V83ApplicationConsumerSettings Settings { get; init; }

        public string Key => Settings.InfobaseUrl;

        public bool Active => Error == null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public IReadOnlyList<string> Topics => Settings.Topics;

        public string InfobaseName { get; init; }

        public string ConsumerGroup { get; private set; }

        public int Consumed { get; private set; }

        public int Saved { get; private set; }

        public Exception? Error { get; private set; }

        private async Task RunConsumerAsync(CancellationToken cancellationToken)
        {
            try
            {
                IConsumer<string, string> consumer = _kafkaService.GetConsumer<string, string>(Settings.Topics, ConsumerGroup);

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
                        Settings,
                        _jsonService,
                        _logger,
                        cancellationToken);

                    if (jsonTransformResults == null || jsonTransformResults.Count == 0)
                    {
                        continue;
                    }

                    await _v77ApplicationSaveTask(
                        jsonTransformResults,
                        Settings,
                        _httpClientFactory,
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

        private static string ExtractInfobaseName(string infobaseUrl) => InfobaseNameRegex().Match(infobaseUrl).Groups[1].Value;

        [GeneratedRegex(@"/([^/]+)")]
        private static partial Regex InfobaseNameRegex();
    }

    public class InstructionNotSpecifiedException : Exception
    {
        internal InstructionNotSpecifiedException(string topic) : base($"Instruction not specified for '{topic}'")
        {
        }
    }
}
