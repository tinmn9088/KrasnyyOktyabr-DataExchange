using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Substracts 2 numbers.
/// </summary>
/// <exception cref="ArgumentNullException"></exception>
public sealed class SubstractExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression) : AbstractBinaryNumberExpression(leftExpression, rightExpression)
{
    protected override Number Calculate(Number left, Number right)
    {
        return left - right;
    }
}
