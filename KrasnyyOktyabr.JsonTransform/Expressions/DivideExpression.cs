using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Divides 2 numbers.
/// </summary>
/// <exception cref="ArgumentNullException"></exception>
public sealed class DivideExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression) : AbstractBinaryNumberExpression(leftExpression, rightExpression)
{
    /// <exception cref="DivideByZeroException"></exception>
    protected override Number Calculate(Number left, Number right)
    {
        return left / right;
    }
}
