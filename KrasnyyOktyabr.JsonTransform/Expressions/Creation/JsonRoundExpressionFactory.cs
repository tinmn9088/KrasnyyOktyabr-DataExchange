using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;
public sealed class JsonRoundExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonUnaryNumberExpressionFactory<RoundExpression>(JsonSchemaPropertyRound, factory)
{
    public static string JsonSchemaPropertyRound => "$round";

    protected override RoundExpression CreateExpressionInstance(IExpression<Task<Number>> valueExpression) => new(valueExpression);
}
