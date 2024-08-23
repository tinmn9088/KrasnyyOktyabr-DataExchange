using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Models;
using KrasnyyOktyabr.ApplicationNet48.Modules.API.Attributes;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;
using KrasnyyOktyabr.DataResolve;
using KrasnyyOktyabr.DataResolve.Resolvers;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.API.Controllers;

[ApiRoutePrefix("resolve")]
public class ResolveController(IComV77ApplicationConnectionFactory connectionFactory) : ApiController
{
    [HttpPost]
    [Route("v77application", Name = "ResolveV77Application")]
    public async Task<IHttpActionResult> ResolveV77ApplicationAsync([FromBody] V77ApplicationResolverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            ConnectionProperties connectionProperties = new(
                infobasePath: request.InfobasePath,
                username: request.Username,
                password: request.Password
            );

            string ertRelativePath = Path.Combine(DataResolveService.DefaultErtRelativePathWithoutName, request.ErtName);

            ComV77ApplicationResolver resolver = request.ErrorMessageName is not null
                ? new(connectionFactory,
                    connectionProperties,
                    ertRelativePath,
                    context: request.FormParams,
                    resultName: request.ResultName,
                    errorMessageName: request.ErrorMessageName)
                : new(connectionFactory,
                    connectionProperties,
                    ertRelativePath,
                    context: request.FormParams,
                    resultName: request.ResultName);

            object ? result = await resolver.ResolveAsync(cancellationToken);

            return result is not null
                ? base.ResponseMessage(new HttpResponseMessage() { Content = new StringContent(result.ToString()) })
                : Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"{ex.GetType().Name}: {ex.Message}");
        }
    }
}
