using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Services.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KrasnyyOktyabr.ApplicationNet48.Health;

public class V83ApplicationProducerServiceHealthChecker(IV83ApplicationProducerService v83ApplicationProducerService) : IHealthCheck
{
    public static string DataKey => "status";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy,
            data: new Dictionary<string, object>()
            {
                { DataKey, v83ApplicationProducerService.Status }
            }));
    }
}
