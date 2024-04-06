using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class RoundExpression(IExpression<Task<Number>> valueExpression) : AbstractUnaryExpression<Number>(valueExpression)
{
    protected override async ValueTask<Number> CalculateAsync(Func<Task<Number>> getValue)
    {
        Number value = await getValue();

        if (value.Int != null)
        {
            return value;
        }

        if (value.Double != null)
        {
            return new Number(Math.Round(value.Double.Value));
        }

        throw new NotImplementedException();
    }
}
