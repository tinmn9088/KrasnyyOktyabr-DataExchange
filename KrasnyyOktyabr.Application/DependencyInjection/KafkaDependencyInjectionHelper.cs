using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using KrasnyyOktyabr.Application.Contracts.Kafka;
using KrasnyyOktyabr.Application.Health;
using KrasnyyOktyabr.Application.Services;
using KrasnyyOktyabr.Application.Services.DataResolve;
using KrasnyyOktyabr.Application.Services.Kafka;

namespace KrasnyyOktyabr.Application.DependencyInjection;

public static class KafkaDependencyInjectionHelper
{
    /// <summary>
    /// Services added:
    ///  <list type="bullet">
    ///   <item><see cref="IOffsetService"/></item>
    ///   <item><see cref="IJsonService"/></item>
    ///   <item><see cref="ITransliterationService"/></item>
    ///   <item><see cref="IKafkaService"/></item>
    ///   <item><see cref="IRestartService"/></item>
    ///   <item>Typed <see cref="HttpClient"/> for <see cref="DataResolveService"/></item>
    /// </list>
    /// 
    /// Called:
    /// <list type="bullet">
    ///   <item><see cref="AddV83ApplicationProducerService(IServiceCollection, IHealthChecksBuilder)"/></item>
    ///   <item><see cref="AddV83ApplicationConsumerService(IServiceCollection, IHealthChecksBuilder)"/></item>
    /// </list>
    ///
    /// When current OS is <c>"windows"</c> are also called:
    /// <list type="bullet">
    ///   <item><see cref="AddV77ApplicationProducerService(IServiceCollection, IHealthChecksBuilder)"/></item>
    ///   <item><see cref="AddV77ApplicationPeriodProduceJobService(IServiceCollection, IHealthChecksBuilder)"/></item>
    ///   <item><see cref="AddV77ApplicationConsumerService(IServiceCollection, IHealthChecksBuilder)"/></item>
    ///   <item><see cref="AddMsSqlConsumerService(IServiceCollection, IHealthChecksBuilder)"/></item>
    /// </list>
    /// </summary>
    public static void AddKafkaClients(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddHttpClient<DataResolveService>();

        services.AddSingleton<IOffsetService, OffsetService>();

        services.AddSingleton<IJsonService, JsonService>();

        services.AddSingleton<ITransliterationService, TransliterationService>();

        services.AddSingleton<IKafkaService, KafkaService>();

        services.AddSingleton<IRestartService, RestartService>();

        services.AddJsonTransform();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            services.AddSingleton<IV77ApplicationLogService, V77ApplicationLogService>();

            services.AddV77ApplicationProducerService(healthChecksBuilder);

            services.AddV77ApplicationPeriodProduceJobService(healthChecksBuilder);

            services.AddV77ApplicationConsumerService(healthChecksBuilder);

            services.AddMsSqlConsumerService(healthChecksBuilder);
        }

        services.AddV83ApplicationProducerService(healthChecksBuilder);

        services.AddV83ApplicationConsumerService(healthChecksBuilder);
    }

    /// <summary>
    /// Register singleton <see cref="IV77ApplicationProducerService"/>, start <see cref="V77ApplicationProducerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void AddV77ApplicationProducerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
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
    /// Register singleton <see cref="IV77ApplicationPeriodProduceJobService"/>, start <see cref="V77ApplicationPeriodProduceJobService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void AddV77ApplicationPeriodProduceJobService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IV77ApplicationPeriodProduceJobService, V77ApplicationPeriodProduceJobService>();

        healthChecksBuilder.AddCheck<V77ApplicationPeriodProduceJobServiceHealthChecker>(nameof(V77ApplicationPeriodProduceJobStatus));
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
        services.AddHostedService(p =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return p.GetRequiredService<IMsSqlConsumerService>();
            }

            throw new NotSupportedException();
        });

        healthChecksBuilder.AddCheck<MsSqlConsumerServiceHealthChecker>(nameof(MsSqlConsumerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IV77ApplicationConsumerService"/>, start <see cref="V77ApplicationConsumerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static void AddV77ApplicationConsumerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IV77ApplicationConsumerService, V77ApplicationConsumerService>();
        services.AddHostedService(p =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return p.GetRequiredService<IV77ApplicationConsumerService>();
            }

            throw new NotSupportedException();
        });

        healthChecksBuilder.AddCheck<V77ApplicationConsumerServiceHealthChecker>(nameof(V77ApplicationConsumerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IV83ApplicationConsumerService"/>, start <see cref="V83ApplicationConsumerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    public static void AddV83ApplicationConsumerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IV83ApplicationConsumerService, V83ApplicationConsumerService>();
        services.AddHostedService(p =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return p.GetRequiredService<IV83ApplicationConsumerService>();
            }

            throw new NotSupportedException();
        });

        healthChecksBuilder.AddCheck<V83ApplicationConsumerServiceHealthChecker>(nameof(V83ApplicationConsumerStatus));
    }
}
