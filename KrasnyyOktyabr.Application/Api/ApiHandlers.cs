using System.Text.Json;
using KrasnyyOktyabr.Application.Services;

namespace KrasnyyOktyabr.Application.Api;

public static class ApiHandlers
{
    public static async ValueTask HandleTestJsonTransform(IJsonService jsonService, HttpContext httpContext, CancellationToken cancellationToken)
    {
        httpContext.Response.ContentType = "application/json";

        try
        {
            await using MemoryStream outputStream = new();

            await jsonService.RunJsonTransformAsync(
                inputStream: httpContext.Request.Body,
                outputStream,
                cancellationToken)
            .ConfigureAwait(false);

            outputStream.Capacity = Convert.ToInt32(outputStream.Length); // Truncate tail of nulls

            await httpContext.Response.Body.WriteAsync(outputStream.GetBuffer(), cancellationToken);
        }
        catch (Exception ex)
        {
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(new Dictionary<string, string>()
            {
                { ex.GetType().Name, ex.Message },
            }), cancellationToken)
            .ConfigureAwait(false);
        }
    }
}
