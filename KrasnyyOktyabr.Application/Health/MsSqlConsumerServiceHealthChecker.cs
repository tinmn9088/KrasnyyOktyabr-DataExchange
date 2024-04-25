using System.Runtime.Versioning;
using KrasnyyOktyabr.Application.Services.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KrasnyyOktyabr.Application.Health;

[SupportedOSPlatform("windows")]
public class MsSqlConsumerServiceHealthChecker(IMsSqlConsumerService msSqlConsumerService) : IHealthCheck
{
    public static string DataKey => "status";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy,
            data: new Dictionary<string, object>()
            {
                { DataKey, msSqlConsumerService.Status }
            }));
    }
}
