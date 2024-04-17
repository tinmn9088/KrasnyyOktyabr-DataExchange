﻿using Confluent.Kafka;

namespace KrasnyyOktyabr.Application.Services.Kafka;

public interface IKafkaService : IRestartable
{
    IProducer<TKey, TValue> GetProducer<TKey, TValue>();

    IConsumer<TKey, TValue> GetConsumer<TKey, TValue>(IEnumerable<string> topics, string consumerGroup);

    /// <summary>
    /// Concatenates all names and transliterates to latin.
    /// </summary>
    string BuildTopicName(params string[] names);

    string ExtractConsumerGroupNameFromConnectionString(string connectionString);
}