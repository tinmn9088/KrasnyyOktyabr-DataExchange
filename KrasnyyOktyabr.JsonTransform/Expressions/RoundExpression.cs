using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class RoundExpression : AbstractUnaryExpression<Number>
{
    private readonly int _digits;

    /// <exception cref="ArgumentException"></exception>
    public RoundExpression(IExpression<Task<Number>> valueExpression, int digits = 0)
        : base(valueExpression)
    {
        if (digits < 0)
        {
            throw new ArgumentException($"Negative digits ({digits}) not allowed");
        }

        _digits = digits;
    }

    protected override async ValueTask<Number> CalculateAsync(Func<Task<Number>> getValue)
    {
        Number value = await getValue();

        if (value.Int != null)
        {
            return value;
        }

        if (value.Double != null)
        {
            return new Number(Math.Round(value.Double.Value, _digits));
        }

        throw new NotImplementedException();
    }
}
