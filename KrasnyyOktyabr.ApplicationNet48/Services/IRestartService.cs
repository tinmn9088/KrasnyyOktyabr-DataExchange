using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.ApplicationNet48.Models;
using Microsoft.Extensions.Hosting;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public interface IRestartService : IHostedService
{
    ValueTask<RestartResult> RestartAsync(CancellationToken cancellationToken);
}
