using System;
using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Modules.API.Attributes;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.CoreServices.PeriodServices;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.API.Controllers;


[ApiRoutePrefix("producers")]
public class ProducersController(IV77ApplicationPeriodProduceJobService service, ILogger<ProducersController> logger) : ApiController
{
    [HttpPost]
    [Route("v77application/jobs/start", Name = "StartV77ApplicationPeriodJob")]
    public IHttpActionResult StartV77ApplicationPeriodJob([FromBody] V77ApplicationPeriodProduceJobRequest request)
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

    [HttpPost]
    [Route("v77application/jobs/cancel", Name = "CancelV77ApplicationPeriodJob")]
    public async Task<IHttpActionResult> CancelV77ApplicationPeriodJob([FromUri] string infobasePath)
    {
        await service.CancelJobAsync(infobasePath);

        return Ok();
    }
}
