#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Logging;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Services.IJsonService;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

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

    /// <summary>
    /// Synchronizes restart methods.
    /// </summary>
    private readonly SemaphoreSlim _restartLock = new(1, 1);

    public int ManagedInstancesCount => _consumers?.Count ?? 0;

    public IStatusContainer<MsSqlConsumerStatus> Status
    {
        get
        {
            if (_consumers == null || _consumers.Count == 0)
            {
                return StatusContainer<MsSqlConsumerStatus>.Empty;
            }

            List<MsSqlConsumerStatus> statuses = new(_consumers.Count);

            foreach (MsSqlConsumer consumer in _consumers.Values)
            {
                statuses.Add(new()
                {
                    ServiceKey = consumer.Key,
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

            return new StatusContainer<MsSqlConsumerStatus>()
            {
                Statuses = statuses
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
            if (_consumers != null && _consumers.TryGetValue(key, out MsSqlConsumer? consumer))
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

            logger.LogJsonTransformResult(jsonTransformResults.Count);
        }
        catch (Exception ex)
        {
            logger.LogJsonTransformError(ex);

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
            if (settings.ConnectionType != null)
            {
                await msSqlService.InsertAsync(settings.ConnectionString, result.Table, result.ColumnValues, settings.ConnectionType.Value);
            }
            else
            {
                await msSqlService.InsertAsync(settings.ConnectionString, result.Table, result.ColumnValues);
            }
        }
    };

    private void StartConsumers()
    {
        MsSqlConsumerSettings[]? producersSettings = GetConsumersSettings();

        if (producersSettings == null)
        {
            logger.LogConfigurationNotFound();

            _consumers = null;

            return;
        }

        logger.LogConfigurationFound(producersSettings.Length);

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
                logger.LogStoppingConsumers(_consumers.Count);
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
            Settings = settings;
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
                logger.LogConsumerGroupNotSpecified();

                ConsumerGroup = DatabaseName;
            }

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _transformMessageTask = transformMessageTask;
            _sqlInsertTask = sqlInsertTask;
            _consumerTask = Task.Run(() => RunConsumerAsync(cancellationToken), cancellationToken);

            LastActivity = DateTimeOffset.Now;
        }

        public MsSqlConsumerSettings Settings { get; private set; }

        public string Key => Settings.ConnectionString;

        public bool Active => Error == null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public IReadOnlyList<string> Topics => Settings.Topics;

        public string DatabaseName { get; private set; }

        public string ConsumerGroup { get; private set; }

        public int Consumed { get; private set; }

        public int Saved { get; private set; }

        public string TablePropertyName => Settings.TablePropertyName;

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

                    _logger.LogConsumedMessage(
                        consumerGroup: ConsumerGroup,
                        topic: consumeResult.Topic,
                        key: consumeResult.Message.Key,
                        length: consumeResult.Message.Value.Length,
                        message: consumeResult.Message.Value);

                    LastActivity = DateTimeOffset.Now;

                    List<JsonTransformMsSqlResult>? jsonTransformResults = await _transformMessageTask(
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

                    await _sqlInsertTask(
                        jsonTransformResults,
                        Settings,
                        _msSqlService,
                        cancellationToken);
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

            _cancellationTokenSource.Cancel();

            try
            {
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
    }

    public class InstructionNotSpecifiedException : Exception
    {
        internal InstructionNotSpecifiedException(string topic) : base($"Instruction not specified for '{topic}'")
        {
        }
    }
}
