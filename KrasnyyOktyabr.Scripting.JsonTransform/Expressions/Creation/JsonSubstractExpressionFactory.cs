using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonSubstractExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryNumberExpressionFactory<SubstractExpression>(JsonSchemaPropertySubstract, factory)
{
    public static string JsonSchemaPropertySubstract => "$substract";

    protected override SubstractExpression CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression) => new(leftExpression, rightExpression);
}
