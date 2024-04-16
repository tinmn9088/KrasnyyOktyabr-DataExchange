using Confluent.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IKafkaService : IRestartable
{
    public IProducer<TKey, TValue> GetProducer<TKey, TValue>();

    /// <summary>
    /// Concatenates all names and transliterates to latin.
    /// </summary>
    public string BuildTopicName(params string[] names);
}
