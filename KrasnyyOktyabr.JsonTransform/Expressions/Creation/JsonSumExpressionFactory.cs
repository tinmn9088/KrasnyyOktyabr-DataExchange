using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonSumExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonNumberExpressionFactory<SumExpression>(JsonSchemaPropertySum, factory)
{
    public static string JsonSchemaPropertySum => "$sum";

    protected override SumExpression CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    {
        return new SumExpression(leftExpression, rightExpression);
    }
}
