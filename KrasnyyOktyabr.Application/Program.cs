using System.Runtime.InteropServices;
using System.Text;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.Application.Services.Kafka;
using KrasnyyOktyabr.ComV77Application;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ServicesStartConcurrently = true;
    options.ServicesStopConcurrently = true;
});

builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IOffsetService, OffsetService>();
builder.Services.AddSingleton<IJsonService, JsonService>();
builder.Services.AddSingleton<ITransliterationService, TransliterationService>();
builder.Services.AddSingleton<IKafkaService, KafkaService>();

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddSingleton<IV77ApplicationLogService, V77ApplicationLogService>();
    builder.Services.AddHostedService<V77ApplicationProducerService>();
    builder.Services.AddSingleton<IComV77ApplicationConnectionFactory, ComV77ApplicationConnection.Factory>();
}

WebApplication app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
