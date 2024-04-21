using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using KrasnyyOktyabr.Application.DependencyInjection;
using KrasnyyOktyabr.Application.Health;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.Application.Services.DataResolve;
using KrasnyyOktyabr.Application.Services.Kafka;
using KrasnyyOktyabr.ComV77Application;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});

IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks();

builder.Services.AddHttpClient<DataResolveService>();

builder.Services.AddSingleton<IOffsetService, OffsetService>();
builder.Services.AddSingleton<IJsonService, JsonService>();
builder.Services.AddSingleton<ITransliterationService, TransliterationService>();
builder.Services.AddSingleton<IKafkaService, KafkaService>();

builder.Services.AddJsonTransform();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddSingleton<IComV77ApplicationConnectionFactory, ComV77ApplicationConnection.Factory>();

    builder.Services.AddSingleton<IMsSqlService, MsSqlService>();

    builder.Services.AddSingleton<IWmiService, WmiService>();

    builder.Services.AddV77ApplicationProducerService(healthChecksBuilder);

    builder.Services.AddV77ApplicationConsumerService(healthChecksBuilder);

    builder.Services.AddMsSqlConsumerService(healthChecksBuilder);
}

// Not implemented yet
// builder.Services.AddV83ApplicationProducerService(healthChecksBuilder);
// builder.Services.AddV83ApplicationConsumerService(healthChecksBuilder);

WebApplication app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = HealthCheckHelper.WebServiceRESTResponseWriter,
});

app.MapPost("/test-json-transform", async (
    [FromBody] Stream requestBody,
    [FromServices] IJsonService jsonService,
    HttpContext httpContext,
    CancellationToken cancellationToken) =>
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
});

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    app.MapGet("/wmi/are-remote-desktop-sessions-allowed", (IWmiService wmiService) => wmiService.AreRdSessionsAllowed());
}

// TODO: return status
app.MapGet("/restart", async (IServiceProvider provider, CancellationToken cancellationToken) =>
{
    async ValueTask Restart<T>() where T : IRestartable
    {
        T? service = provider.GetService<T>();

        if (service != null)
        {
            await service.RestartAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        await Restart<IV77ApplicationProducerService>().ConfigureAwait(false);

        await Restart<IV77ApplicationConsumerService>().ConfigureAwait(false);
    }

    await Restart<IMsSqlConsumerService>().ConfigureAwait(false);

    // Not implemented yet
    // await Restart<IV83ApplicationProducerService>().ConfigureAwait(false);
    // await Restart<IV83ApplicationConsumerService>().ConfigureAwait(false);
});

app.Run();
