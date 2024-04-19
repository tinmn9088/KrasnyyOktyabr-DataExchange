using KrasnyyOktyabr.Application.Services.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KrasnyyOktyabr.Application.Health;

public class V83ApplicationConsumerServiceHealthChecker(IV83ApplicationConsumerService v83ApplicationConsumerService) : IHealthCheck
{
    public static string DataKey => "producers";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy,
            data: new Dictionary<string, object>()
            {
                { DataKey, v83ApplicationConsumerService.Status }
            }));
    }
}
