using System.Runtime.Versioning;
using KrasnyyOktyabr.ComV77Application;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KrasnyyOktyabr.Application.Health;

[SupportedOSPlatform("windows")]
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
