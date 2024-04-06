namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractUnaryBoolExpression : AbstractExpression<Task<bool>>
{
    private readonly IExpression<Task<bool>> _valueExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractUnaryBoolExpression(IExpression<Task<bool>> valueExpression)
    {
        ArgumentNullException.ThrowIfNull(valueExpression);

        _valueExpression = valueExpression;
    }

    public override async Task<bool> InnerInterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        bool value = await _valueExpression.InterpretAsync(context, cancellationToken);

        return Calculate(value);
    }

    protected abstract bool Calculate(bool value);
}
