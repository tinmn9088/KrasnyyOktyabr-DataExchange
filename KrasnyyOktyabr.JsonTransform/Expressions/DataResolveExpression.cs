using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class DataResolveExpression : AbstractExpression<Task<object?>>
{
    private readonly IExpression<Task<string>> _resolverExpression;

    private readonly IExpression<Task<Dictionary<string, object?>>> _argsExpression;

    private readonly IDataResolveService _dataResolveService;

    /// <exception cref="ArgumentNullException"></exception>
    public DataResolveExpression(
        IExpression<Task<string>> resolverExpression,
        IExpression<Task<Dictionary<string, object?>>> argsExpression,
        IDataResolveService dataResolveService)
    {
        ArgumentNullException.ThrowIfNull(resolverExpression);
        ArgumentNullException.ThrowIfNull(argsExpression);
        ArgumentNullException.ThrowIfNull(dataResolveService);

        _resolverExpression = resolverExpression;
        _argsExpression = argsExpression;
        _dataResolveService = dataResolveService;
    }

    /// <exception cref="IDataResolveService.ResolverNotFoundException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    protected override async Task<object?> InnerInterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string resolver = await _resolverExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

        Dictionary<string, object?> args = await _argsExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false) ?? throw new NullReferenceException();

        return await _dataResolveService.ResolveAsync(resolver, args, cancellationToken).ConfigureAwait(false);
    }
}
