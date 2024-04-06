namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class NotExpression(IExpression<Task<bool>> valueExpression) : AbstractUnaryExpression<bool>(valueExpression)
{
    protected override async ValueTask<bool> CalculateAsync(Func<Task<bool>> getValue) => !await getValue();
}
