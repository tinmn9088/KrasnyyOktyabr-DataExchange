using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonIsGreaterExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryExpressionFactory<Number, Number, IsGreaterExpression>(JsonSchemaPropertyGt, factory)
{
    public static string JsonSchemaPropertyGt => "$gt";

    protected override IsGreaterExpression CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression) => new(leftExpression, rightExpression);
}
