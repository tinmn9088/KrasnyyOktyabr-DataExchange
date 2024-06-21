#nullable enable

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Models;
using KrasnyyOktyabr.ApplicationNet48.Services.DataResolve;
using KrasnyyOktyabr.ComV77Application;
using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

[RoutePrefix("api/resolve/v77application")]
public class V77ApplicationResolverController(IComV77ApplicationConnectionFactory connectionFactory) : ApiController
{
    [Route("")]
    [HttpPost]
    public async Task<IHttpActionResult> ResolveAsync([FromBody] V77ApplicationResolverRequest request, CancellationToken cancellationToken)
    {
        try
        {
            ConnectionProperties connectionProperties = new(
                infobasePath: request.InfobasePath,
                username: request.Username,
                password: request.Password
            );

            string ertRelativePath = Path.Combine(DataResolveService.DefaultErtRelativePathWithoutName, request.ErtName);

            ComV77ApplicationResolver resolver = new(connectionFactory, connectionProperties, ertRelativePath, request.FormParams, request.ResultName);

            object? result = await resolver.ResolveAsync(cancellationToken);

            return base.ResponseMessage(new HttpResponseMessage()
            {
                Content = new StringContent(result?.ToString()),
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"{ex.GetType().Name}: {ex.Message}");
        }
    }
}
