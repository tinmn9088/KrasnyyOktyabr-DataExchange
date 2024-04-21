using System.Runtime.InteropServices;
using System.Text;
using KrasnyyOktyabr.Application.Api;
using KrasnyyOktyabr.Application.DependencyInjection;
using KrasnyyOktyabr.Application.Health;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.ComV77Application;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance); // To read strings in 'Windows-1251' encoding


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddSingleton<IComV77ApplicationConnectionFactory, ComV77ApplicationConnection.Factory>();

    builder.Services.AddSingleton<IMsSqlService, MsSqlService>();

    builder.Services.AddSingleton<IWmiService, WmiService>();
}

builder.Services.AddMemoryCache();

IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks();

builder.Services.AddKafkaClients(healthChecksBuilder);


WebApplication app = builder.Build();

RouteGroupBuilder apiV0 = app.MapGroup("/api/v0");

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    apiV0.MapGet("/wmi/are-remote-desktop-sessions-allowed", (IWmiService wmiService) => wmiService.AreRdSessionsAllowed());
}

apiV0.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = HealthCheckHelper.WebServiceRESTResponseWriter,
});

apiV0.MapPost("/test-json-transform", async (
    [FromServices] IJsonService jsonService,
    HttpContext httpContext,
    CancellationToken cancellationToken)
    => await ApiHandlers.HandleTestJsonTransform(jsonService, httpContext, cancellationToken).ConfigureAwait(false));

apiV0.MapGet("/restart", async (
    IServiceProvider provider,
    IRestartService restartService,
    CancellationToken cancellationToken)
    => await restartService.RestartAsync(provider, cancellationToken));

app.Run();
