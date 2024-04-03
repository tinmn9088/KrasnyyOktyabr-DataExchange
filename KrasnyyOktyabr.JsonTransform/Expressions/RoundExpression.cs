using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class RoundExpression(IExpression<Task<Number>> valueExpression) : AbstractUnaryNumberExpression(valueExpression)
{
    protected override Number Calculate(Number value)
    {
        if (value.Int != null)
        {
            return value;
        }

        if (value.Double != null)
        {
            return new(Math.Round(value.Double.Value));
        }

        throw new NotImplementedException();
    }
}
