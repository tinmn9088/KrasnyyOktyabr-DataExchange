using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.CoreServices.ConsumerServices;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HealthCheckers;

public class V77ApplicationConsumerServiceHealthChecker(IV77ApplicationConsumerService v77ApplicationConsumerService) : IHealthCheck
{
    public static string DataKey => "status";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy,
            data: new Dictionary<string, object>()
            {
                { DataKey, v77ApplicationConsumerService.Status }
            }));
    }
}
