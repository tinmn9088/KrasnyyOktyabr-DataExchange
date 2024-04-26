using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ComV77Application;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KrasnyyOktyabr.ApplicationNet48.Health;

public class ComV77ApplicationConnectionFactoryHealthChecker(IComV77ApplicationConnectionFactory comV77ApplicationConnectionFactory) : IHealthCheck
{
    public static string DataKey => "connections";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new HealthCheckResult(HealthStatus.Healthy,
            data: new Dictionary<string, object>()
            {
                { DataKey, comV77ApplicationConnectionFactory.Status }
            }));
    }
}
