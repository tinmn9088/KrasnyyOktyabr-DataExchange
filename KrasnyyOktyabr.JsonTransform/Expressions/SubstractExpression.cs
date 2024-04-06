using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Substracts 2 numbers.
/// </summary>
/// <exception cref="ArgumentNullException"></exception>
public sealed class SubstractExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    : AbstractBinaryExpression<Number>(leftExpression, rightExpression)
{
    protected override ValueTask<Number> CalculateAsync(Number left, Number right)
    {
        return ValueTask.FromResult(left - right);
    }
}
