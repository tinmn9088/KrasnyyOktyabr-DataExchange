namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonNotExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonUnaryBoolExpressionFactory<NotExpression>(JsonSchemaPropertyNot, factory)
{
    public static string JsonSchemaPropertyNot => "$not";

    protected override NotExpression CreateExpressionInstance(IExpression<Task<bool>> valueExpression) => new(valueExpression);
}
