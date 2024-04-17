using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using KrasnyyOktyabr.Application.Health;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.Application.Services.Kafka;
using static KrasnyyOktyabr.Application.Services.Kafka.IMsSqlConsumerService;
using static KrasnyyOktyabr.Application.Services.Kafka.IV77ApplicationProducerService;
using static KrasnyyOktyabr.Application.Services.Kafka.IV83ApplicationProducerService;

namespace KrasnyyOktyabr.Application.DependencyInjection;

public static class KafkaDependencyInjectionHelper
{
    /// <summary>
    /// Register singleton <see cref="IV77ApplicationProducerService"/>, start <see cref="V77ApplicationProducerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
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

        healthChecksBuilder.AddCheck<V77ApplicationProducerServiceHealthChecker>(nameof(V77ApplicationProducerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IV83ApplicationProducerService"/>, start <see cref="V83ApplicationProducerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    public static void AddV83ApplicationProducerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IV83ApplicationProducerService, V83ApplicationProducerService>();
        services.AddHostedService(p =>
        {
            return p.GetRequiredService<IV83ApplicationProducerService>();
        });

        healthChecksBuilder.AddCheck<V83ApplicationProducerServiceHealthChecker>(nameof(V83ApplicationProducerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IMsSqlConsumerService"/>, start <see cref="MsSqlConsumerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void AddMsSqlConsumerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IMsSqlConsumerService, MsSqlConsumerService>();

        // To make it possible to inject this hosted service through DI
        services.AddSingleton<IMsSqlConsumerService, MsSqlConsumerService>();
        services.AddHostedService(p =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return p.GetRequiredService<IMsSqlConsumerService>();
            }

            throw new NotSupportedException();
        });

        healthChecksBuilder.AddCheck<MsSqlConsumerServiceHealthChecker>(nameof(MsSqlProducerStatus));
    }
}
