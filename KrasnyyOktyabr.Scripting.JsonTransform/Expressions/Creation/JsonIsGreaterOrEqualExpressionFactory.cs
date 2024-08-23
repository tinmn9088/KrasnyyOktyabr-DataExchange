using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonIsGreaterOrEqualExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryExpressionFactory<Number, Number, IsGreaterOrEqualExpression>(JsonSchemaPropertyGte, factory)
{
    public static string JsonSchemaPropertyGte => "$gte";

    protected override IsGreaterOrEqualExpression CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression) => new(leftExpression, rightExpression);
}
