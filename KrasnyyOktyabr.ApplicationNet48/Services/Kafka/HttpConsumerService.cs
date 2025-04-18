#nullable enable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Logging.KafkaLoggingHelper;
using static KrasnyyOktyabr.ApplicationNet48.Services.HttpClientHelper;
using static KrasnyyOktyabr.ApplicationNet48.Services.TimeHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

public sealed partial class HttpConsumerService(
    IConfiguration configuration,
    IJsonService jsonService,
    IHttpClientFactory httpClientFactory,
    IKafkaService kafkaService,
    ITransliterationService transliterationService,
    ILogger<HttpConsumerService> logger,
    ILoggerFactory loggerFactory)
    : IHttpConsumerService
{
    public delegate ValueTask<List<string>?> TransformMessageAsync(
        string topic,
        string message,
        HttpConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken);

    public delegate ValueTask HttpSendAsync(
        List<string> jsonTransformResults,
        HttpConsumerSettings settings,
        IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken);

    /// <summary>
    /// <para>
    /// Is <c>null</c> when no configuration found.
    /// </para>
    /// <para>
    /// Keys are results of <see cref="HttpConsumer.Key"/>.
    /// </para>
    /// </summary>
    private Dictionary<string, HttpConsumer>? _consumers;

    /// <summary>
    /// Synchronizes restart methods.
    /// </summary>
    private readonly SemaphoreSlim _restartLock = new(1, 1);

    public int ManagedInstancesCount => _consumers?.Count ?? 0;

    public IStatusContainer<HttpConsumerStatus> Status
    {
        get
        {
            if (_consumers is null || _consumers.Count == 0)
            {
                return StatusContainer<HttpConsumerStatus>.Empty;
            }

            List<HttpConsumerStatus> statuses = new(_consumers.Count);

            foreach (HttpConsumer consumer in _consumers.Values)
            {
                statuses.Add(new()
                {
                    ServiceKey = consumer.Key,
                    Active = consumer.Active,
                    LastActivity = consumer.LastActivity,
                    ErrorMessage = consumer.Error?.Message,
                    Url = consumer.Url,
                    Consumed = consumer.Consumed,
                    Sent = consumer.Saved,
                    Topics = consumer.Topics,
                    ConsumerGroup = consumer.ConsumerGroup,
                    SuspendSchedule = consumer.Settings.SuspendSchedule,
                });
            }

            return new StatusContainer<HttpConsumerStatus>()
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
            if (_consumers is not null && _consumers.TryGetValue(key, out HttpConsumer? consumer))
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
        HttpConsumerSettings settings,
        IJsonService jsonService,
        ILogger logger,
        CancellationToken cancellationToken) =>
    {
        if (!settings.TopicsInstructionNames.TryGetValue(topic, out string? instructionName))
        {
            throw new InstructionNotSpecifiedException(topic);
        }

        List<string> jsonTransformResults = await jsonService.RunJsonTransformOnConsumedMessageAsync(
            instructionName,
            message,
            cancellationToken);

        logger.LogJsonTransformResult(jsonTransformResults.Count);

        return jsonTransformResults;
    };

    public HttpSendAsync HttpSendTask => async (
        List<string> jsonTransformResults,
        HttpConsumerSettings settings,
        IHttpClientFactory httpClientFactory,
        CancellationToken cancellationToken) =>
    {
        using HttpClient httpClient = httpClientFactory.CreateClient();

        foreach (string result in jsonTransformResults)
        {
            HttpRequestMessage request = new(HttpMethod.Post, settings.Url)
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json"),
            };

            if (settings.Username is not null)
            {
                request.Headers.Authorization = GetAuthenticationHeaderValue(settings.Username, settings.Password);
            }

            HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new FailedToSendException(content);
            }
        }
    };

    private void StartConsumers()
    {
        HttpConsumerSettings[]? producersSettings = GetConsumersSettings();

        if (producersSettings is null)
        {
            logger.LogConfigurationNotFound();

            _consumers = null;

            return;
        }

        logger.LogConfigurationFound(producersSettings.Length);

        foreach (HttpConsumerSettings settings in producersSettings)
        {
            StartConsumer(settings);
        }
    }

    private HttpConsumerSettings[]? GetConsumersSettings()
        => ValidationHelper.GetAndValidateKafkaClientSettings<HttpConsumerSettings>(configuration, HttpConsumerSettings.Position, logger);

    /// <summary>
    /// Creates new <see cref="HttpConsumer"/> and saves it to <see cref="_consumers"/>.
    /// </summary>
    private void StartConsumer(HttpConsumerSettings settings)
    {
        _consumers ??= [];
        HttpConsumer consumer = new(
            loggerFactory.CreateLogger<HttpConsumer>(),
            settings,
            kafkaService,
            jsonService,
            httpClientFactory,
            transliterationService,
            TransformMessageTask,
            HttpSendTask);

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

            foreach (HttpConsumer consumer in _consumers.Values)
            {
                await consumer.DisposeAsync();
            }

            _consumers.Clear();
        }
    }

    private sealed partial class HttpConsumer : IAsyncDisposable
    {
        private readonly ILogger<HttpConsumer> _logger;

        private readonly IKafkaService _kafkaService;

        private readonly IJsonService _jsonService;

        private readonly IHttpClientFactory _httpClientFactory;

        private readonly TransformMessageAsync _transformMessageTask;

        private readonly HttpSendAsync _httpSendTask;

        private readonly Task _consumerTask;

        /// <remarks>
        /// Need to be disposed.
        /// </remarks>
        private readonly CancellationTokenSource _cancellationTokenSource;

        internal HttpConsumer(
            ILogger<HttpConsumer> logger,
            HttpConsumerSettings settings,
            IKafkaService kafkaService,
            IJsonService jsonService,
            IHttpClientFactory httpClientFactory,
            ITransliterationService transliterationService,
            TransformMessageAsync transformMessageTask,
            HttpSendAsync httpSendTask)
        {
            _logger = logger;
            Settings = settings;
            _kafkaService = kafkaService;
            _jsonService = jsonService;
            _httpClientFactory = httpClientFactory;

            Url = settings.Url;

            if (settings.ConsumerGroup is not null)
            {
                ConsumerGroup = settings.ConsumerGroup;
            }
            else
            {
                logger.LogConsumerGroupNotSpecified();

                ConsumerGroup = transliterationService.TransliterateToLatin(Url);
            }

            _cancellationTokenSource = new();
            CancellationToken cancellationToken = _cancellationTokenSource.Token;

            _transformMessageTask = transformMessageTask;
            _httpSendTask = httpSendTask;
            _consumerTask = Task.Run(() => RunConsumerAsync(cancellationToken), cancellationToken);

            LastActivity = DateTimeOffset.Now;
        }

        public HttpConsumerSettings Settings { get; private set; }

        public string Key => Settings.Url;

        public bool Active => Error is null;

        public DateTimeOffset LastActivity { get; private set; }

        public bool CancellationRequested => _cancellationTokenSource.IsCancellationRequested;

        public IReadOnlyList<string> Topics => [.. Settings.TopicsInstructionNames.Keys];

        public string Url { get; private set; }

        public string ConsumerGroup { get; private set; }

        public int Consumed { get; private set; }

        public int Saved { get; private set; }

        public Exception? Error { get; private set; }

        private async Task RunConsumerAsync(CancellationToken cancellationToken)
        {
            try
            {
                using IConsumer<string, string> consumer = _kafkaService.GetConsumer<string, string>(Topics, ConsumerGroup);

                while (!cancellationToken.IsCancellationRequested)
                {
                    LastActivity = DateTimeOffset.Now;

                    if (Settings.SuspendSchedule is not null)
                    {
                        await WaitPeriodsEndAsync(() => DateTimeOffset.Now, Settings.SuspendSchedule, cancellationToken, _logger);
                    }

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

                    await _httpSendTask(
                        jsonTransformResults,
                        Settings,
                        _httpClientFactory,
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
    }

    public class InstructionNotSpecifiedException : Exception
    {
        internal InstructionNotSpecifiedException(string topic) : base($"Instruction not specified for '{topic}'")
        {
        }
    }

    public class FailedToSendException : Exception
    {
        internal FailedToSendException(string message) : base(message)
        {
        }
    }
}
