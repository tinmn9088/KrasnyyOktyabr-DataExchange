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
using Microsoft.Extensions.Logging;
using static KrasnyyOktyabr.ApplicationNet48.Controllers.ControllersHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

[RoutePrefix("api/kafka")]
public class KafkaController(IKafkaService kafkaService, ILogger<KafkaController> logger) : ApiController
{
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
