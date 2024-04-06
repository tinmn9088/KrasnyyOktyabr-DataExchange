namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class NotExpression(IExpression<Task<bool>> valueExpression) : AbstractUnaryExpression<bool>(valueExpression)
{
    protected override ValueTask<bool> Calculate(bool value) => ValueTask.FromResult(!value);
}
