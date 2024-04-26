#nullable enable

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Services;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

public class JsonTransformController(IJsonService jsonService) : ApiController
{
    [HttpPost]
    public async Task<IHttpActionResult> Run(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        MemoryStream resultStream = new();

        Stream bodyStream = await request.Content.ReadAsStreamAsync();

        if (bodyStream == null)
        {
            return BadRequest();
        }

        bodyStream.Position = 0; // Stream has been read and position is in the end now

        try
        {
            await jsonService.RunJsonTransformAsync(bodyStream, resultStream, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

        response.Content = new JsonTransformContent(resultStream);

        return RunResult(response);
    }

    private static JsonTransformRunResult RunResult(HttpResponseMessage response) => new(response);

    public class JsonTransformRunResult(HttpResponseMessage response) : IHttpActionResult
    {
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) => Task.FromResult(response);
    }

    public class JsonTransformContent : HttpContent
    {
        private readonly MemoryStream _bodyStream;

        internal JsonTransformContent(MemoryStream bodyStream)
        {
            bodyStream.Capacity = Convert.ToInt32(bodyStream.Length); // Truncate tail of nulls

            _bodyStream = bodyStream;

            Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
        {
            await stream.WriteAsync(_bodyStream.GetBuffer(), 0, Convert.ToInt32(_bodyStream.Length)).ConfigureAwait(false);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = 0;
            return false;
        }
    }
}
