using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Multiplies 2 numbers.
/// </summary>
/// <exception cref="ArgumentNullException"></exception>
public sealed class MultiplyExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    : AbstractBinaryExpression<Number>(leftExpression, rightExpression)
{
    protected override async ValueTask<Number> CalculateAsync(Func<Task<Number>> getLeft, Func<Task<Number>> getRight) =>
        await getLeft().ConfigureAwait(false) * await getRight().ConfigureAwait(false);
}
