using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using KrasnyyOktyabr.Application.Health;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.Application.Services.Kafka;
using KrasnyyOktyabr.ComV77Application;

namespace KrasnyyOktyabr.Application.DependencyInjection;

public static class KafkaDependencyInjectionHelper
{
    [SupportedOSPlatform("windows")]
    public static void AddV77ApplicationProducerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IV77ApplicationLogService, V77ApplicationLogService>();

        // To make it possible to inject this hosted service through DI
        services.AddSingleton<IV77ApplicationProducerService, V77ApplicationProducerService>();
        services.AddHostedService(p =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return p.GetRequiredService<IV77ApplicationProducerService>();
            }

            throw new NotSupportedException();
        });

        services.AddSingleton<IComV77ApplicationConnectionFactory, ComV77ApplicationConnection.Factory>();

        healthChecksBuilder.AddCheck<V77ApplicationProducerServiceHealthChecker>(nameof(V77ApplicationProducerService));
    }
}
