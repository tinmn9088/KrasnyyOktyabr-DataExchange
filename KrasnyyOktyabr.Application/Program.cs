using System.Runtime.InteropServices;
using System.Text;
using KrasnyyOktyabr.Application.Api;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using KrasnyyOktyabr.Application.DependencyInjection;
using KrasnyyOktyabr.Application.Health;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.Application.Services.Kafka;
using KrasnyyOktyabr.ComV77Application;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Web;

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    // Setup encodings to read strings in 'Windows-1251' encoding
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // Setup logging
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Setup hosted services
    builder.Services.Configure<HostOptions>(options =>
    {
        options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        options.ServicesStartConcurrently = true;
        options.ServicesStopConcurrently = true;
    });

    // Setup Windows-only services
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        builder.Services.AddSingleton<IComV77ApplicationConnectionFactory, ComV77ApplicationConnection.Factory>();

        builder.Services.AddSingleton<IMsSqlService, MsSqlService>();

        builder.Services.AddSingleton<IWmiService, WmiService>();
    }

    builder.Services.AddMemoryCache();

    // Setup health checks
    IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks();

    // Setup Kafka clients
    builder.Services.AddKafkaClients(healthChecksBuilder);

    // Setup restart service
    builder.Services.AddSingleton<IRestartService, RestartService>();
    builder.Services.AddHostedService(provider => provider.GetRequiredService<IRestartService>());

    builder.Services.AddExceptionHandler<ExceptionHandler>();


    // Setup endpoints
    WebApplication app = builder.Build();

    app.UseExceptionHandler(_ => { /* Use lambda to prevent a bug with 'ExceptionHandlingPath' */ });

    // Legacy endpoint
    app.MapHealthChecks("/HealthService.svc/Status", new HealthCheckOptions()
    {
        ResponseWriter = HealthCheckHelper.WebServiceRESTResponseWriter,
    });

    RouteGroupBuilder apiV0 = app.MapGroup("/api/v0");

    apiV0.MapHealthChecks("/health", new HealthCheckOptions()
    {
        ResponseWriter = HealthCheckHelper.WebServiceRESTResponseWriter,
    });

    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        apiV0.MapGet("/wmi/are-remote-desktop-sessions-allowed", (IWmiService wmiService) => wmiService.AreRdSessionsAllowed());

        apiV0.MapPost("/producers/v77application/jobs/start", (
            [FromBody] V77ApplicationPeriodProduceJobRequest request,
            [FromServices] IV77ApplicationPeriodProduceJobService service,
            CancellationToken cancellationToken)
            => service.StartJob(request));

        apiV0.MapPost("/producers/v77application/jobs/cancel", async (
            [FromQuery] string infobasePath,
            [FromServices] IV77ApplicationPeriodProduceJobService service)
            => await service.CancelJobAsync(infobasePath));
    }

    apiV0.MapPost("/test-json-transform", async (
        [FromServices] IJsonService jsonService,
        HttpContext httpContext,
        CancellationToken cancellationToken)
        => await ApiHandlers.HandleTestJsonTransform(jsonService, httpContext, cancellationToken).ConfigureAwait(false));

    apiV0.MapGet("/restart", async (
        IRestartService restartService,
        CancellationToken cancellationToken)
        => await restartService.RestartAsync(cancellationToken));

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped");
    throw;
}
finally
{
    LogManager.Shutdown();
}
