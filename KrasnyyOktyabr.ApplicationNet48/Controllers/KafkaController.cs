#nullable enable

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Logging;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Controllers.ControllersHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

[RoutePrefix("api/kafka")]
public class KafkaController(IKafkaService kafkaService, IMemoryCache cache, ILogger<KafkaController> logger) : ApiController
{
    public class CacheKeys
    {
        public static string Producers => nameof(KafkaController) + "_" + nameof(Producers);
    }

    [Route("produce")]
    [HttpPost]
    public async Task<IHttpActionResult> ProduceMessage(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            string topic = GetRequiredQueryParameter(request, "topic");
            string? key = GetOptionalQueryParameter(request, "key");

            using Stream bodyStream = await request.Content.ReadAsStreamAsync();

            if (bodyStream is null || bodyStream.Length == 0)
            {
                throw new ArgumentException("Missing body");
            }

            using StreamReader reader = new(bodyStream);

            IProducer<string?, string> producer = GetProducer();

            Message<string?, string> message = new()
            {
                Key = key,
                Value = await reader.ReadToEndAsync().ConfigureAwait(false),
            };

            await producer.ProduceAsync(topic, message, cancellationToken).ConfigureAwait(false);

            logger.LogProducedMessage(topic, message.Key, message.Value);

            return Ok();
        }
        catch (OperationCanceledException)
        {
            logger.LogOperationCancelled();

            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <returns>Kafka producer (do not dispose).</returns>
    /// <exception cref="InvalidCastException"></exception>
    private IProducer<string?, string> GetProducer()
    {
        if (cache.TryGetValue(CacheKeys.Producers, out object? producer))
        {
            return producer as IProducer<string?, string>
                ?? throw new InvalidCastException($"Cached value was not of type '{typeof(IProducer<string?, string>)}'");
        }

        IProducer<string?, string> newInstance = kafkaService.GetProducer<string?, string>();

        logger.LogDebug($"New instance of Kafka producer is created ({newInstance.GetHashCode()})");

        MemoryCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(3),
        };

        options.RegisterPostEvictionCallback(new PostEvictionDelegate((key, value, reason, state) =>
        {
            logger.LogDebug($"'{typeof(PostEvictionDelegate).Name}' called for producer ({value?.GetHashCode()})");

            IProducer<string?, string>? producer = value as IProducer<string?, string>;

            if (producer is not null)
            {
                producer.Dispose();

                logger.LogDebug($"Producer ({value?.GetHashCode()}) disposed");
            }
        }));

        cache.Set(CacheKeys.Producers, newInstance, options);

        return newInstance;
    }
}
