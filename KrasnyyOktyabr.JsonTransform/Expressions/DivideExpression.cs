using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Divides 2 numbers.
/// </summary>
/// <exception cref="ArgumentNullException"></exception>
public sealed class DivideExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    : AbstractBinaryExpression<Number>(leftExpression, rightExpression)
{
    /// <exception cref="DivideByZeroException"></exception>
    protected override async ValueTask<Number> CalculateAsync(Func<Task<Number>> getLeft, Func<Task<Number>> getRight)
    {
        return await getLeft() / await getRight();
    }
}
