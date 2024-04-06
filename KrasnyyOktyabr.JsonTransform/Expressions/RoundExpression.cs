using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class RoundExpression(IExpression<Task<Number>> valueExpression) : AbstractUnaryExpression<Number>(valueExpression)
{
    protected override ValueTask<Number> Calculate(Number value)
    {
        if (value.Int != null)
        {
            return ValueTask.FromResult(value);
        }

        if (value.Double != null)
        {
            return ValueTask.FromResult(new Number(Math.Round(value.Double.Value)));
        }

        throw new NotImplementedException();
    }
}
