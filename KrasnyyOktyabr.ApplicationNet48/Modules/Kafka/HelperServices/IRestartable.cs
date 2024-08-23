using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.HelperServices;

public interface IRestartable
{
    ValueTask RestartAsync(CancellationToken cancellationToken);

    ValueTask RestartAsync(string key, CancellationToken cancellationToken);
}
