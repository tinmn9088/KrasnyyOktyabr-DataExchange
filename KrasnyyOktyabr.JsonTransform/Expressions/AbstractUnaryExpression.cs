namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractUnaryExpression<TValue, TResult> : AbstractExpression<Task<TResult>>
{
    private readonly IExpression<Task<TValue>> _valueExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractUnaryExpression(IExpression<Task<TValue>> valueExpression)
    {
        ArgumentNullException.ThrowIfNull(valueExpression);

        _valueExpression = valueExpression;
    }

    protected override async Task<TResult> InnerInterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        async Task<TValue> getValue() => await _valueExpression.InterpretAsync(context, cancellationToken);

        return await CalculateAsync(getValue);
    }

    protected abstract ValueTask<TResult> CalculateAsync(Func<Task<TValue>> value);
}

public abstract class AbstractUnaryExpression<T>(IExpression<Task<T>> valueExpression) : AbstractUnaryExpression<T, T>(valueExpression)
{
}
