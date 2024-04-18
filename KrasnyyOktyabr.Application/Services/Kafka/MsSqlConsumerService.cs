using System.Runtime.Versioning;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.Application.Logging;
using static KrasnyyOktyabr.Application.Services.IJsonService;
using static KrasnyyOktyabr.Application.Services.Kafka.IMsSqlConsumerService;

namespace KrasnyyOktyabr.Application.Services.Kafka;

[SupportedOSPlatform("windows")]
public sealed class MsSqlConsumerService(
    IConfiguration configuration,
    IJsonService jsonService,
    IMsSqlService msSqlService,
    IKafkaService kafkaService,
    ILogger<MsSqlConsumerService> logger,
    ILoggerFactory loggerFactory)
    : IMsSqlConsumerService
{
    public delegate ValueTask<List<JsonTransformMsSqlResult>?> TransformMessageAsync(
        string topic,
        string message,
        MsSqlConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken);

    public delegate ValueTask SqlInsertAsync(
        List<JsonTransformMsSqlResult> jsonTransformResults,
        MsSqlConsumerSettings settings,
        IMsSqlService msSqlService,
        CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    /// Is <c>null</c> when no configuration found.
    /// </para>
    /// <para>
    /// Keys are results of <see cref="V77ApplicationProducer.Key"/>.
    /// </para>
    /// </summary>
    private Dictionary<string, MsSqlConsumer>? _consumers;

    public List<MsSqlProducerStatus> Status
    {
        get
        {
            if (_consumers == null || _consumers.Count == 0)
            {
                return [];
            }

            List<MsSqlProducerStatus> statuses = new(_consumers.Count);

            foreach (MsSqlConsumer consumer in _consumers.Values)
            {
                statuses.Add(new()
                {
                    Active = consumer.Active,
                    LastActivity = consumer.LastActivity,
                    ErrorMessage = consumer.Error?.Message,
                    Consumed = consumer.Consumed,
                    Saved = consumer.Saved,
                    TablePropertyName = consumer.TablePropertyName,
                    Topics = consumer.Topics,
                    ConsumerGroup = consumer.ConsumerGroup,
                });
            }

            return statuses;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Starting ...");

        StartConsumers();

        return Task.CompletedTask;
    }

    public async Task RestartAsync(CancellationToken cancellationToken)
    {
        logger.LogTrace("Restarting ...");

        await StopConsumersAsync();

        StartConsumers();
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
        MsSqlConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken) =>
    {
        List<JsonTransformMsSqlResult> jsonTransformResults;

        try
        {
            if (!settings.TopicsInstructionNames.TryGetValue(topic, out string? instructionName))
            {
                throw new InstructionNotSpecifiedException(topic);
            }

            jsonTransformResults = await jsonService.RunJsonTransformOnConsumedMessageMsSqlAsync(
                instructionName,
                message,
                settings.TablePropertyName,
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

    public SqlInsertAsync SqlInsertTask => async (
        List<JsonTransformMsSqlResult> jsonTransformResults,
        MsSqlConsumerSettings settings,
        IMsSqlService msSqlService,
        CancellationToken cancellationToken) =>
    {
        foreach (JsonTransformMsSqlResult result in jsonTransformResults)
        {
            await msSqlService.InsertAsync(settings.ConnectionString, result.Table, result.ColumnValues);
        }
    };

    private void StartConsumers()
    {
        MsSqlConsumerSettings[]? producersSettings = GetConsumersSettings();

        if (producersSettings == null)
        {
            logger.ConfigurationNotFound();

            _consumers = null;

            return;
        }

        logger.ConfigurationFound(producersSettings.Length);

        foreach (MsSqlConsumerSettings settings in producersSettings)
        {
            StartConsumer(settings);
        }
    }

    private MsSqlConsumerSettings[]? GetConsumersSettings()
        => ValidationHelper.GetAndValidateKafkaClientSettings<MsSqlConsumerSettings>(configuration, MsSqlConsumerSettings.Position, logger);

    /// <summary>
    /// Creates new <see cref="MsSqlConsumer"/> and saves it to <see cref="_producers"/>.
    /// </summary>
    private void StartConsumer(MsSqlConsumerSettings settings)
    {
        _consumers ??= [];
        MsSqlConsumer consumer = new(
            loggerFactory.CreateLogger<MsSqlConsumer>(),
            settings,
            kafkaService,
            jsonService,
            msSqlService,
            TransformMessageTask,
            SqlInsertTask);

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

            foreach (MsSqlConsumer consumer in _consumers.Values)
            {
                await consumer.DisposeAsync();
            }

            _consumers.Clear();
        }
    }

    private sealed class MsSqlConsumer : IAsyncDisposable
    {
        private readonly ILogger<MsSqlConsumer> _logger;

        private readonly MsSqlConsumerSettings _settings;

        private readonly IKafkaService _kafkaService;

        private readonly IJsonService _jsonService;

        private readonly IMsSqlService _msSqlService;

        private readonly TransformMessageAsync _transformMessageTask;

        private readonly SqlInsertAsync _sqlInsertTask;

        private readonly Task _consumerTask;

        /// <remarks>
        /// Need to be disposed.
        /// </remarks>
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal MsSqlConsumer(
            ILogger<MsSqlConsumer> logger,
            MsSqlConsumerSettings settings,
            IKafkaService kafkaService,
            IJsonService jsonService,
            IMsSqlService msSqlService,
            TransformMessageAsync transformMessageTask,
            SqlInsertAsync sqlInsertTask)
        {
            _logger = logger;
            _settings = settings;
            _kafkaService = kafkaService;
            _jsonService = jsonService;
            _msSqlService = msSqlService;

            DatabaseName = _kafkaService.ExtractConsumerGroupNameFromConnectionString(settings.ConnectionString);

            if (settings.ConsumerGroup != null)
            {
                ConsumerGroup = settings.ConsumerGroup;
            }
            else
            {
                logger.ConsumerGroupNotPresent();

                ConsumerGroup = DatabaseName;
            }

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _transformMessageTask = transformMessageTask;
            _sqlInsertTask = sqlInsertTask;
            _consumerTask = Task.Run(() => RunConsumerAsync(cancellationToken), cancellationToken);

            LastActivity = DateTimeOffset.Now;
        }

        public string Key => _settings.ConnectionString;

        public bool Active => Error == null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public IReadOnlyList<string> Topics => _settings.Topics;

        public string DatabaseName { get; init; }

        public string ConsumerGroup { get; private set; }

        public int Consumed { get; private set; }

        public int Saved { get; private set; }

        public string TablePropertyName => _settings.TablePropertyName;

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
                        shortenedMessage: KafkaLoggingHelper.ShortenMessage(consumeResult.Message.Value, 400));

                    LastActivity = DateTimeOffset.Now;

                    List<JsonTransformMsSqlResult>? jsonTransformResults = await _transformMessageTask(
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

                    await _sqlInsertTask(
                        jsonTransformResults,
                        _settings,
                        _msSqlService,
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
            _logger.Disposing(_settings.ConnectionString);

            _cancellationTokenSource.Cancel();

            await _consumerTask.ConfigureAwait(false);
        }
    }

    public class InstructionNotSpecifiedException : Exception
    {
        internal InstructionNotSpecifiedException(string topic) : base($"Instruction not specified for '{topic}'")
        {
        }
    }
}
