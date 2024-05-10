namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractUnaryExpression<TValue, TResult>(IExpression<Task<TValue>> valueExpression) : AbstractExpression<Task<TResult>>
{
    private readonly IExpression<Task<TValue>> _valueExpression = valueExpression ?? throw new ArgumentNullException(nameof(valueExpression));

    public override async Task<TResult> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            async Task<TValue> getValue() => await _valueExpression.InterpretAsync(context, cancellationToken).ConfigureAwait(false);

            return await CalculateAsync(getValue).ConfigureAwait(false);
        }
        catch (InterpretException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InterpretException(ex.Message, Mark);
        }
    }

    protected abstract ValueTask<TResult> CalculateAsync(Func<Task<TValue>> value);
}

public abstract class AbstractUnaryExpression<T>(IExpression<Task<T>> valueExpression) : AbstractUnaryExpression<T, T>(valueExpression)
{
}
