using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Modules.API.Attributes;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.API.Controllers;

[ApiRoutePrefix("restart")]
public class RestartController(IRestartService restartService, ILogger<RestartController> logger) : ApiController
{
    [HttpGet]
    [Route("", Name = "ExecuteRestart")]
    public async Task<IHttpActionResult> GetRestartResult()
    {
        try
        {
            return Json(await restartService.RestartAsync(CancellationToken.None).ConfigureAwait(false),
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error on restart");

            return InternalServerError(ex);
        }
    }
}
