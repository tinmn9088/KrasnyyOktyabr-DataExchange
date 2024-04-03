using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractUnaryNumberExpression : AbstractExpression<Task<Number>>
{
    private readonly IExpression<Task<Number>> _valueExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractUnaryNumberExpression(IExpression<Task<Number>> valueExpression)
    {
        ArgumentNullException.ThrowIfNull(valueExpression);

        _valueExpression = valueExpression;
    }

    public override async Task<Number> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        Number value = await _valueExpression.InterpretAsync(context, cancellationToken);

        return Calculate(value);
    }

    protected abstract Number Calculate(Number value);
}
