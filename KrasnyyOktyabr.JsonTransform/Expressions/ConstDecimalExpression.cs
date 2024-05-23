namespace KrasnyyOktyabr.JsonTransform.Expressions;

public sealed class ConstDecimalExpression(decimal value) : AbstractConstExpression<decimal>(value)
{
}
