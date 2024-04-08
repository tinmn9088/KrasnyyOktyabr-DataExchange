using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class IsGreaterExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    : AbstractBinaryExpression<Number, Number, bool>(leftExpression, rightExpression)
{
    protected override async ValueTask<bool> CalculateAsync(Func<Task<Number>> getLeft, Func<Task<Number>> getRight) =>
        await getLeft().ConfigureAwait(false) > await getRight().ConfigureAwait(false);
}
