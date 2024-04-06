using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Summarizes 2 numbers.
/// </summary>
/// <exception cref="ArgumentNullException"></exception>
public sealed class SumExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    : AbstractBinaryExpression<Number>(leftExpression, rightExpression)
{
    protected override async ValueTask<Number> CalculateAsync(Func<Task<Number>> getLeft, Func<Task<Number>> getRight)
    {
        return await getLeft() + await getRight();
    }
}
