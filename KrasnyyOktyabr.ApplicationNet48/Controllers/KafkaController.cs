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
using Microsoft.Extensions.Primitives;
using static KrasnyyOktyabr.ApplicationNet48.Controllers.ControllersHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

[RoutePrefix("api/kafka")]
public class KafkaController(IKafkaService kafkaService, IMemoryCache cache, ILogger<KafkaController> logger) : ApiController
{
    public class CacheKeys
    {
        public static string Producers => nameof(KafkaController) + "_" + nameof(Producers);
    }

    private TimeSpan CacheTimeout => TimeSpan.FromMinutes(5);

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
        if (cache.TryGetValue(CacheKeys.Producers, out (IProducer<string?, string> producer, CancellationTokenSource expirationTokenSource)? cacheItem))
        {
            cacheItem!.Value.expirationTokenSource.CancelAfter(CacheTimeout);
            return cacheItem!.Value.producer;
        }

        IProducer<string?, string> newInstance = kafkaService.GetProducer<string?, string>();

        logger.LogDebug("New instance of Kafka producer is created ({Hash})", newInstance.GetHashCode());

        MemoryCacheEntryOptions options = new()
        {
            SlidingExpiration = CacheTimeout,
        };

        options.RegisterPostEvictionCallback(new PostEvictionDelegate((key, value, reason, state) =>
        {
            logger.LogDebug("'{CallbackName}' called for on cache entry '{Key}'", typeof(PostEvictionDelegate).Name, key);

            if (value is (IProducer<string?, string>, CancellationTokenSource))
            {
                (IProducer<string?, string> producer, CancellationTokenSource expirationTokenSource) = ((IProducer<string?, string>, CancellationTokenSource))value;

                producer.Dispose();
                expirationTokenSource.Dispose();

                logger.LogDebug("Producer ({Hash}) disposed", producer.GetHashCode());
            }
        }));

        CancellationTokenSource expirationTokenSource = new(CacheTimeout);
        options.AddExpirationToken(new CancellationChangeToken(expirationTokenSource.Token));

        cache.Set(CacheKeys.Producers, (newInstance, expirationTokenSource), options);

        return newInstance;
    }
}
