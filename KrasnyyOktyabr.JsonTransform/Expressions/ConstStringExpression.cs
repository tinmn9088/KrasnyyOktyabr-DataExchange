namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ConstStringExpression(string value) : AbstractConstExpression<string>(value)
{
}
