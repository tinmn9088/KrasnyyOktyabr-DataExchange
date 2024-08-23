using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.DataResolve;

public interface IDataResolver
{
    ValueTask<object> ResolveAsync(CancellationToken cancellationToken);
}
