using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using KrasnyyOktyabr.ApplicationNet48.Services;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

public class RestartController(IRestartService restartService, ILogger<RestartController> logger) : ApiController
{
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
