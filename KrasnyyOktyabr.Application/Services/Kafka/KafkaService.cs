using System.ComponentModel.DataAnnotations;
using System.Text;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.Application.Logging;
namespace KrasnyyOktyabr.Application.Services.Kafka;

public sealed class KafkaService : IKafkaService
{
    private readonly ILogger<KafkaService> _logger;

    private readonly IConfiguration _configuration;

    private readonly ITransliterationService _transliterationService;

    private KafkaSettings? _settings;

    public KafkaService(IConfiguration configuration, ITransliterationService transliterationService, ILogger<KafkaService> logger)
    {
        _logger = logger;
        _configuration = configuration;
        _transliterationService = transliterationService;

        LoadKafkaSettings();
    }

    public Task RestartAsync(CancellationToken cancellationToken)
    {
        LoadKafkaSettings();

        return Task.CompletedTask;
    }

    public IProducer<TKey, TValue> GetProducer<TKey, TValue>()
    {
        ProducerConfig config = new()
        {
            BootstrapServers = _settings!.Socket,
            MessageMaxBytes = _settings.MessageMaxBytes,
        };

        return new ProducerBuilder<TKey, TValue>(config).Build();
    }

    /// <summary>Updates <see cref="_settings"/>.</summary>
    /// <remarks>
    /// <see cref="_configuration"/> and <see cref="_logger"/> need to be initialized.
    /// </remarks>
    /// <exception cref="ArgumentNullException"></exception>
    private void LoadKafkaSettings()
    {
        _settings = _configuration
                .GetSection(KafkaSettings.Position)
                .Get<KafkaSettings>();

        if (_settings == null)
        {
            _logger.ConfigurationNotFound();

            return;
        }

        try
        {
            ValidationHelper.ValidateObject(_settings);
        }
        catch (ValidationException ex)
        {
            _logger.InvalidConfiguration(ex, KafkaSettings.Position);
        }
    }

    public string BuildTopicName(params string[] names)
    {
        StringBuilder stringBuilder = new();

        for (int i = 0; i < names.Length; i++)
        {
            if (i != 0)
            {
                stringBuilder.Append('_');
            }

            stringBuilder.Append(names[i]);
        }

        return _transliterationService.TransliterateToLatin(stringBuilder.ToString());
    }
}
