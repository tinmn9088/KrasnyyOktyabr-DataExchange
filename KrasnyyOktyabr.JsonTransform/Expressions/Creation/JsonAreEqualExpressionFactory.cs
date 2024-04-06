namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonAreEqualExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryExpressionFactory<object?, object?, AreEqualExpression>(JsonSchemaPropertyEq, factory)
{
    public static string JsonSchemaPropertyEq => "$eq";

    protected override AreEqualExpression CreateExpressionInstance(IExpression<Task<object?>> leftExpression, IExpression<Task<object?>> rightExpression) => new(leftExpression, rightExpression);
}
