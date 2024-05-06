#nullable enable

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.DependencyInjection;
using KrasnyyOktyabr.ApplicationNet48.Services;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;
using KrasnyyOktyabr.ComV77Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace KrasnyyOktyabr.ApplicationNet48;

public class WebApiApplication : HttpApplication
{
    private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

    private CancellationTokenSource? _hostCancellation;

    private Task? _hostTask;

    protected void Application_Start()
    {
        try
        {
            // Setup encodings to read strings in 'Windows-1251' encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            IHost app = BuildHost();

            GlobalConfiguration.Configure(WebApiConfig.Register(provider: app.Services));

            _hostCancellation = new();
            _hostTask = Task.Run(() =>
            {
                try
                {
                    app.RunAsync(_hostCancellation.Token);
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex);
                }
            });
        }
        catch (Exception ex)
        {
            s_logger.Error(ex);

            throw;
        }
    }

    protected void Application_End()
    {
        _hostCancellation?.Cancel();
        _hostTask?.Wait();

        _hostCancellation = null;
        _hostTask = null;
    }

    public override void Dispose()
    {
        base.Dispose();

        _hostCancellation?.Cancel();
        _hostTask?.Wait();
    }

    private IHost BuildHost()
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        string configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");

        builder.Configuration.AddJsonFile(configurationPath, optional: false, reloadOnChange: true);

        // Setup logging
        builder.Logging.ClearProviders();
        builder.Logging.AddNLogWeb();

        // Setup hosted services
        builder.Services.Configure<HostOptions>(options =>
        {
            options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            options.ServicesStartConcurrently = true;
            options.ServicesStopConcurrently = true;
        });

        builder.Services.AddSingleton<IComV77ApplicationConnectionFactory, ComV77ApplicationConnection.Factory>();

        builder.Services.AddSingleton<IMsSqlService, MsSqlService>();

        builder.Services.AddSingleton<IWmiService, WmiService>();

        builder.Services.AddSingleton<IJsonService, JsonService>();

        builder.Services.AddSingleton<ITransliterationService, TransliterationService>();

        builder.Services.AddSingleton<IKafkaService, KafkaService>();

        builder.Services.AddControllers();

        builder.Services.AddMemoryCache();

        // Setup Kafka clients
        IHealthChecksBuilder healthChecksBuilder = builder.Services.AddHealthChecks();
        builder.Services.AddKafkaClients(healthChecksBuilder);

        // Setup restart service
        builder.Services.AddSingleton<IRestartService, RestartService>();
        builder.Services.AddHostedService(provider => provider.GetRequiredService<IRestartService>());

        return builder.Build();
    }
}
