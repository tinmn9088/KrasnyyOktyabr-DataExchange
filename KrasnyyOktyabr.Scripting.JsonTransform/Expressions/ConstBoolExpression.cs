namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ConstBoolExpression(bool value) : AbstractConstExpression<bool>(value)
{
}
