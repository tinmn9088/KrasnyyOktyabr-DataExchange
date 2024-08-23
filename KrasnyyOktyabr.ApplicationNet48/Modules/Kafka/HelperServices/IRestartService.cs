using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Models;
using Microsoft.Extensions.Hosting;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;

public interface IRestartService : IHostedService
{
    ValueTask<RestartResult> RestartAsync(CancellationToken cancellationToken);
}
