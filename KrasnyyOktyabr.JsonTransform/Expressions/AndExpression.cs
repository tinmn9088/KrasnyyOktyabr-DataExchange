namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class AndExpression(IExpression<Task<bool>> leftExpression, IExpression<Task<bool>> rightExpression)
    : AbstractBinaryExpression<bool>(leftExpression, rightExpression)
{
    protected override ValueTask<bool> CalculateAsync(bool left, bool right) => ValueTask.FromResult(left && right);
}
