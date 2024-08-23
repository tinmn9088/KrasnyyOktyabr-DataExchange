#nullable enable

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Confluent.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Common.Logging;
using KrasnyyOktyabr.ApplicationNet48.Modules.API.Attributes;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.CoreServices;
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Modules.API.Helpers.ControllersHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.API.Controllers;

[ApiRoutePrefix("kafka")]
public class KafkaController(IKafkaService kafkaService, ILogger<KafkaController> logger) : ApiController
{
    [HttpPost]
    [Route("produce", Name = "ProduceKafkaMessage")]
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

            using IProducer<string?, string> producer = kafkaService.GetProducer<string?, string>();

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
}
