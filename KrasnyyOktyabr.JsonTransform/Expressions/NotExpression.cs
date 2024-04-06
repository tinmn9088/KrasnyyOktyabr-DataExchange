namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class NotExpression(IExpression<Task<bool>> valueExpression) : AbstractUnaryBoolExpression(valueExpression)
{
    protected override bool Calculate(bool value) => !value;
}
