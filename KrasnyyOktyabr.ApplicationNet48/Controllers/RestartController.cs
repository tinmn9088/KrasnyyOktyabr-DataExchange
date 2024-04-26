using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using KrasnyyOktyabr.ApplicationNet48.Services;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

public class RestartController(IRestartService restartService) : ApiController
{
    public async Task<IHttpActionResult> GetRestartResult(CancellationToken cancellationToken)
    {
        try
        {
            return Json(await restartService.RestartAsync(cancellationToken).ConfigureAwait(false),
                new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
        }
        catch (Exception ex)
        {
            return InternalServerError(ex);
        }
    }
}
