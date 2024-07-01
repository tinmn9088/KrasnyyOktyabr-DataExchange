#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Services.Kafka.V77ApplicationHelper;
using static KrasnyyOktyabr.ApplicationNet48.Logging.KafkaLoggingHelper;
using static KrasnyyOktyabr.ApplicationNet48.Services.TimeHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public sealed class V77ApplicationConsumerService(
    IConfiguration configuration,
    IWmiService wmiService,
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
            if (_consumers is null || _consumers.Count == 0)
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
                    SuspendSchedule = consumer.Settings.SuspendSchedule,
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
        logger.LogStarting();

        try
        {
            StartConsumers();

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
            await StopConsumersAsync();

            StartConsumers();

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
            if (_consumers is not null && _consumers.TryGetValue(key, out V77ApplicationConsumer? consumer))
            {
                _consumers.Remove(key);

                StartConsumer(consumer.Settings);

                logger.LogRestarted(key);
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

        await StopConsumersAsync();

        logger.LogStopped();
    }

    public async ValueTask DisposeAsync()
    {
        logger.LogDisposing();

        await StopConsumersAsync();

        logger.LogDisposed();
    }

    public TransformMessageAsync TransformMessageTask => async (
        string topic,
        string message,
        V77ApplicationConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken) =>
    {
        if (!settings.TopicsInstructionNames.TryGetValue(topic, out string? instructionName))
        {
            throw new InstructionNotSpecifiedException(topic);
        }

        List<string> jsonTransformResults = await jsonService.RunJsonTransformOnConsumedMessageVApplicationAsync(
            instructionName,
            message,
            cancellationToken);

        logger.LogJsonTransformResult(jsonTransformResults.Count);

        return jsonTransformResults;
    };

    public V77ApplicationSaveAsync V77ApplicationSaveTask => async (
        List<string> jsonTransformResults,
        V77ApplicationConsumerSettings settings,
        IComV77ApplicationConnectionFactory connectionFactory,
        CancellationToken cancellationToken) =>
    {
        string infobaseFullPath = GetInfobaseFullPath(settings.InfobasePath);

        logger.LogSavingObjects(jsonTransformResults.Count);

        ConnectionProperties connectionProperties = new(
            infobasePath: infobaseFullPath,
            username: settings.Username ?? string.Empty,
            password: settings.Password
        );

        await using (IComV77ApplicationConnection connection = await connectionFactory.GetConnectionAsync(connectionProperties, cancellationToken).ConfigureAwait(false))
        {
            await connection.ConnectAsync(cancellationToken).ConfigureAwait(false);

            foreach (string result in jsonTransformResults)
            {
                Dictionary<string, string> ertContext = new()
                {
                    { "ObjectJson", result },
                };

                object? error = await connection.RunErtAsync(
                    ertRelativePath: GetErtRelativePath(settings),
                    ertContext,
                    resultName: "Error",
                    cancellationToken).ConfigureAwait(false);

                if (error is not null)
                {
                    throw new FailedToSaveObjectException(error.ToString());
                }
            }
        }
    };

    private void StartConsumers()
    {
        V77ApplicationConsumerSettings[]? producersSettings = GetConsumersSettings();

        if (producersSettings is null)
        {
            logger.LogConfigurationNotFound();

            _consumers = null;

            return;
        }

        logger.LogConfigurationFound(producersSettings.Length);

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
            wmiService,
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
        if (_consumers is not null)
        {
            if (_consumers.Count > 0)
            {
                logger.LogStoppingConsumers(_consumers.Count);
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

        private readonly IWmiService _wmiService;

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
            IWmiService wmiService,
            IKafkaService kafkaService,
            IJsonService jsonService,
            IComV77ApplicationConnectionFactory connectionFactory,
            ITransliterationService transliterationService,
            TransformMessageAsync transformMessageTask,
            V77ApplicationSaveAsync v77ApplicationSaveTask)
        {
            _logger = logger;
            Settings = settings;
            _wmiService = wmiService;
            _kafkaService = kafkaService;
            _jsonService = jsonService;
            _connectionFactory = connectionFactory;

            InfobaseName = ExtractInfobaseName(settings.InfobasePath);

            if (settings.ConsumerGroup is not null)
            {
                ConsumerGroup = settings.ConsumerGroup;
            }
            else
            {
                logger.LogConsumerGroupNotSpecified();

                ConsumerGroup = transliterationService.TransliterateToLatin(InfobaseName);
            }

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _transformMessageTask = transformMessageTask;
            _v77ApplicationSaveTask = v77ApplicationSaveTask;
            _consumerTask = Task.Run(() => RunConsumerAsync(cancellationToken), cancellationToken);

            LastActivity = DateTimeOffset.Now;
        }

        public V77ApplicationConsumerSettings Settings { get; private set; }

        public string Key => Settings.InfobasePath;

        public bool Active => Error is null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public IReadOnlyList<string> Topics => Settings.Topics;

        public string InfobaseName { get; private set; }

        public string ConsumerGroup { get; private set; }

        public int Consumed { get; private set; }

        public int Saved { get; private set; }

        public Exception? Error { get; private set; }

        private async Task RunConsumerAsync(CancellationToken cancellationToken)
        {
            try
            {
                using IConsumer<string, string> consumer = _kafkaService.GetConsumer<string, string>(Settings.Topics, ConsumerGroup);

                while (!cancellationToken.IsCancellationRequested)
                {
                    LastActivity = DateTimeOffset.Now;

                    if (Settings.SuspendSchedule is not null)
                    {
                        await WaitPeriodsEndAsync(() => DateTimeOffset.Now, Settings.SuspendSchedule, cancellationToken, _logger);
                    }

                    await WaitRdSessionsAllowed(_wmiService, cancellationToken, _logger);

                    ConsumeResult<string, string> consumeResult = consumer.Consume(cancellationToken);

                    Consumed++;

                    _logger.LogConsumedMessage(
                        ConsumerGroup,
                        topic: consumeResult.Topic,
                        key: consumeResult.Message.Key,
                        length: consumeResult.Message.Value.Length,
                        message: consumeResult.Message.Value);

                    LastActivity = DateTimeOffset.Now;

                    List<string>? jsonTransformResults = await _transformMessageTask(
                        topic: consumeResult.Topic,
                        message: consumeResult.Message.Value,
                        Settings,
                        _jsonService,
                        _logger,
                        cancellationToken);

                    LastActivity = DateTimeOffset.Now;

                    if (jsonTransformResults is null || jsonTransformResults.Count == 0)
                    {
                        consumer.Commit();

                        LastActivity = DateTimeOffset.Now;

                        continue;
                    }

                    await _v77ApplicationSaveTask(
                        jsonTransformResults,
                        Settings,
                        _connectionFactory,
                        cancellationToken);

                    Saved += jsonTransformResults.Count;

                    LastActivity = DateTimeOffset.Now;

                    consumer.Commit();

                    LastActivity = DateTimeOffset.Now;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogOperationCancelled();
            }
            catch (Exception ex)
            {
                Error = ex;

                _logger.LogConsumerError(ex);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _logger.LogDisposing(Key);

            try
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();

                await _consumerTask.ConfigureAwait(false);
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

        private static string ExtractInfobaseName(string infobasePath) => Path.GetFileName(infobasePath);
    }

    private static string GetInfobaseFullPath(string infobasePath) => Path.GetFullPath(infobasePath);

    private static string GetErtRelativePath(V77ApplicationConsumerSettings settings) => settings.ErtRelativePath ?? DefaultConsumerErtRelativePath;

    public class InstructionNotSpecifiedException : Exception
    {
        internal InstructionNotSpecifiedException(string topic) : base($"Instruction not specified for '{topic}'")
        {
        }
    }

    public class FailedToSaveObjectException : Exception
    {
        internal FailedToSaveObjectException(string message) : base(message)
        {
        }
    }
}
