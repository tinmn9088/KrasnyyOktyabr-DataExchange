using System.Threading;
using System.Threading.Tasks;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public interface IOffsetService
{
#nullable enable
    Task<string?> GetOffset(string key, CancellationToken cancellationToken = default);
#nullable disable

    Task CommitOffset(string key, string offset, CancellationToken cancellationToken = default);
}
