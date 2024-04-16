using System.Runtime.InteropServices;
using System.Text;
using KrasnyyOktyabr.Application.DependencyInjection;
using KrasnyyOktyabr.Application.Health;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.Application.Services.Kafka;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});

IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IOffsetService, OffsetService>();
builder.Services.AddSingleton<IJsonService, JsonService>();
builder.Services.AddSingleton<ITransliterationService, TransliterationService>();
builder.Services.AddSingleton<IKafkaService, KafkaService>();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddV77ApplicationProducerService(healthChecksBuilder);
}

WebApplication app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions()
{
    ResponseWriter = HealthCheckHelper.WebServiceRESTResponseWriter,
});

app.Run();
