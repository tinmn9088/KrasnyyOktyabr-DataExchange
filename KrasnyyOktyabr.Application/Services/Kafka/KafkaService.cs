using System.Text;
using Confluent.Kafka;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public sealed class KafkaService(IConfiguration configuration, ITransliterationService transliterationService) : IKafkaService
{
    private readonly IConfiguration _configuration = configuration;

    private KafkaSettings _settings = GetKafkaSettings(configuration);

    public Task RestartAsync(CancellationToken cancellationToken)
    {
        _settings = GetKafkaSettings(_configuration);

        return Task.CompletedTask;
    }

    public IProducer<TKey, TValue> GetProducer<TKey, TValue>()
    {
        ProducerConfig config = new()
        {
            BootstrapServers = _settings.Socket,
            MessageMaxBytes = _settings.MessageMaxBytes,
        };

        return new ProducerBuilder<TKey, TValue>(config).Build();
    }

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="KafkaSettingsNotFound"></exception>
    private static KafkaSettings GetKafkaSettings(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return configuration
            .GetSection(KafkaSettings.Position)
            .Get<KafkaSettings>()
            ?? throw new KafkaSettingsNotFound(KafkaSettings.Position);
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

        return transliterationService.TransliterateToLatin(stringBuilder.ToString());
    }

    public class KafkaSettingsNotFound : Exception
    {
        internal KafkaSettingsNotFound(string position)
            : base($"Position '{position}'")
        {
        }
    }
}
