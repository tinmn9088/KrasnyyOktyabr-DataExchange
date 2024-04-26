using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

[RoutePrefix("api/producers/v77application/jobs")]
public class V77ApplicationPeriodProduceJobController(IV77ApplicationPeriodProduceJobService service) : ApiController
{
    [Route("start")]
    [HttpPost]
    public IHttpActionResult StartJob([FromBody] V77ApplicationPeriodProduceJobRequest request)
    {
        service.StartJob(request);

        return Ok();
    }

    [Route("cancel")]
    [HttpPost]
    public async Task<IHttpActionResult> CancelJob([FromUri] string infobasePath)
    {
        await service.CancelJobAsync(infobasePath);

        return Ok();
    }
}
