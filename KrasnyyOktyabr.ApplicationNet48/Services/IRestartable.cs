using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public interface IRestartable
{
    ValueTask RestartAsync(CancellationToken cancellationToken);

    ValueTask RestartAsync(string key, CancellationToken cancellationToken);
}
