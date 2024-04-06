namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class AndExpression(IExpression<Task<bool>> leftExpression, IExpression<Task<bool>> rightExpression)
    : AbstractBinaryExpression<bool>(leftExpression, rightExpression)
{
    protected override async ValueTask<bool> CalculateAsync(Func<Task<bool>> getLeft, Func<Task<bool>> getRight) => await getLeft() && await getRight();
}
