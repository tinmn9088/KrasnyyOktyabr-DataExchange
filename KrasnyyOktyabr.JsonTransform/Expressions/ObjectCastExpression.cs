namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="object?"/>.
/// </summary>
public sealed class ObjectCastExpression : IExpression<Task<object?>>
{
    private readonly IExpression<Task> _innerExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public ObjectCastExpression(IExpression<Task> inner)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _innerExpression = inner;
    }

    public async Task<object?> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        return await CastExpressionsHelper.ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken));
    }
}
