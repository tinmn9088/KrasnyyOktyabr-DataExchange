using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMultiplyExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonNumberExpressionFactory<MultiplyExpression>(JsonSchemaPropertyMultiply, factory)
{
    public static string JsonSchemaPropertyMultiply => "$multiply";

    protected override MultiplyExpression CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    {
        return new MultiplyExpression(leftExpression, rightExpression);
    }
}
