﻿#nullable enable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Logging.KafkaLoggingHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

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
            if (_consumers != null && _consumers.TryGetValue(key, out V83ApplicationConsumer? consumer))
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

            logger.LogJsonTransformResult(jsonTransformResults.Count);
        }
        catch (Exception ex)
        {
            logger.LogJsonTransformError(ex);

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
            logger.LogConfigurationNotFound();

            _consumers = null;

            return;
        }

        logger.LogConfigurationFound(producersSettings.Length);

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
                logger.LogStoppingConsumers(_consumers.Count);
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

        public V83ApplicationConsumerSettings Settings { get; private set; }

        public string Key => Settings.InfobaseUrl;

        public bool Active => Error == null;

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
                IConsumer<string, string> consumer = _kafkaService.GetConsumer<string, string>(Settings.Topics, ConsumerGroup);

                while (!cancellationToken.IsCancellationRequested)
                {
                    LastActivity = DateTimeOffset.Now;

                    ConsumeResult<string, string> consumeResult = consumer.Consume(cancellationToken);

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

            await _consumerTask.ConfigureAwait(false);

            _logger.LogDisposed(Key);
        }

        private static readonly Regex s_infobaseNameRegex = new(@"/([^/]+)");

        private static string ExtractInfobaseName(string infobaseUrl) => s_infobaseNameRegex.Match(infobaseUrl).Groups[1].Value;
    }

    public class InstructionNotSpecifiedException : Exception
    {
        internal InstructionNotSpecifiedException(string topic) : base($"Instruction not specified for '{topic}'")
        {
        }
    }
}