namespace KrasnyyOktyabr.Application.Services;

public interface IOffsetService
{
    Task<string?> GetOffset(string key, CancellationToken cancellationToken = default);

    Task CommitOffset(string key, string offset, CancellationToken cancellationToken = default);
}
