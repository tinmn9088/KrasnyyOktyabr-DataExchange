using KrasnyyOktyabr.ApplicationNet48.Health;
using KrasnyyOktyabr.ApplicationNet48.Models.Kafka;
using KrasnyyOktyabr.ApplicationNet48.Services;
using KrasnyyOktyabr.ApplicationNet48.Services.DataResolve;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;
using Microsoft.Extensions.DependencyInjection;
using static KrasnyyOktyabr.ComV77Application.IComV77ApplicationConnectionFactory;

namespace KrasnyyOktyabr.ApplicationNet48.DependencyInjection;

public static class KafkaDependencyInjectionHelper
{
    /// <summary>
    /// Services added:
    ///  <list type="bullet">
    ///   <item><see cref="IOffsetService"/></item>
    ///   <item><see cref="IRestartService"/></item>
    ///   <item>Typed <see cref="HttpClient"/> for <see cref="DataResolveService"/></item>
    /// </list>
    /// 
    /// Called:
    /// <list type="bullet">
    ///   <item><see cref="AddV83ApplicationProducerService(IServiceCollection, IHealthChecksBuilder)"/></item>
    ///   <item><see cref="AddHttpConsumerService(IServiceCollection, IHealthChecksBuilder)"/></item>
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

        services.AddSingleton<IRestartService, RestartService>();

        services.AddJsonTransform();

        services.AddSingleton<IV77ApplicationLogService, V77ApplicationLogService>();

        healthChecksBuilder.AddCheck<ComV77ApplicationConnectionFactoryHealthChecker>(nameof(ComV77ApplicationConnectionFactoryStatus));

        services.AddV77ApplicationProducerService(healthChecksBuilder);

        services.AddV77ApplicationPeriodProduceJobService(healthChecksBuilder);

        services.AddV77ApplicationConsumerService(healthChecksBuilder);

        services.AddMsSqlConsumerService(healthChecksBuilder);

        services.AddV83ApplicationProducerService(healthChecksBuilder);

        services.AddHttpConsumerService(healthChecksBuilder);
    }

    /// <summary>
    /// Register singleton <see cref="IV77ApplicationProducerService"/>, start <see cref="V77ApplicationProducerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    public static void AddV77ApplicationProducerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IV77ApplicationProducerService, V77ApplicationProducerService>();
        services.AddHostedService(p => p.GetRequiredService<IV77ApplicationProducerService>());

        healthChecksBuilder.AddCheck<V77ApplicationProducerServiceHealthChecker>(nameof(V77ApplicationProducerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IV77ApplicationPeriodProduceJobService"/>, start <see cref="V77ApplicationPeriodProduceJobService"/>
    /// as hosted service and add health check for it.
    /// </summary>
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
        services.AddHostedService(p => p.GetRequiredService<IV83ApplicationProducerService>());

        healthChecksBuilder.AddCheck<V83ApplicationProducerServiceHealthChecker>(nameof(V83ApplicationProducerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IMsSqlConsumerService"/>, start <see cref="MsSqlConsumerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    public static void AddMsSqlConsumerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IMsSqlConsumerService, MsSqlConsumerService>();
        services.AddHostedService(p => p.GetRequiredService<IMsSqlConsumerService>());

        healthChecksBuilder.AddCheck<MsSqlConsumerServiceHealthChecker>(nameof(MsSqlConsumerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IV77ApplicationConsumerService"/>, start <see cref="V77ApplicationConsumerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    public static void AddV77ApplicationConsumerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IV77ApplicationConsumerService, V77ApplicationConsumerService>();
        services.AddHostedService(p => p.GetRequiredService<IV77ApplicationConsumerService>());

        healthChecksBuilder.AddCheck<V77ApplicationConsumerServiceHealthChecker>(nameof(V77ApplicationConsumerStatus));
    }

    /// <summary>
    /// Register singleton <see cref="IHttpConsumerService"/>, start <see cref="HttpConsumerService"/>
    /// as hosted service and add health check for it.
    /// </summary>
    public static void AddHttpConsumerService(this IServiceCollection services, IHealthChecksBuilder healthChecksBuilder)
    {
        services.AddSingleton<IHttpConsumerService, HttpConsumerService>();
        services.AddHostedService(p => p.GetRequiredService<IHttpConsumerService>());

        healthChecksBuilder.AddCheck<HttpConsumerServiceHealthChecker>(nameof(HttpConsumerStatus));
    }
}
