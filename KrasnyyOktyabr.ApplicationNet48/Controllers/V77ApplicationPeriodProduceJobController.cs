using System;
using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

[RoutePrefix("api/producers/v77application/jobs")]
public class V77ApplicationPeriodProduceJobController(IV77ApplicationPeriodProduceJobService service, ILogger<V77ApplicationPeriodProduceJobController> logger) : ApiController
{
    [Route("start")]
    [HttpPost]
    public IHttpActionResult StartJob([FromBody] V77ApplicationPeriodProduceJobRequest request)
    {
        try
        {
            service.StartJob(request);

            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to start period produce job");

            return BadRequest(ex.Message);
        }
    }

    [Route("cancel")]
    [HttpPost]
    public async Task<IHttpActionResult> CancelJob([FromUri] string infobasePath)
    {
        await service.CancelJobAsync(infobasePath);

        return Ok();
    }
}
