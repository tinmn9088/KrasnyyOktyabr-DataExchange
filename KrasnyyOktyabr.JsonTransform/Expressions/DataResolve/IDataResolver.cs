namespace KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

public interface IDataResolver
{
    ValueTask<object?> ResolveAsync(CancellationToken cancellationToken);
}
