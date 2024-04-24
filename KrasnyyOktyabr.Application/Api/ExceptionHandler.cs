using Microsoft.AspNetCore.Diagnostics;

namespace KrasnyyOktyabr.Application.Api;

public class ExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = 400;

        await httpContext.Response.WriteAsJsonAsync(new
        {
            exception.Message,
        }, cancellationToken);

        return true;
    }
}
