using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonDivideExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonNumberExpressionFactory<DivideExpression>(JsonSchemaPropertyDiv, factory)
{
    public static string JsonSchemaPropertyDiv => "$div";

    protected override DivideExpression CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    {
        return new DivideExpression(leftExpression, rightExpression);
    }
}
