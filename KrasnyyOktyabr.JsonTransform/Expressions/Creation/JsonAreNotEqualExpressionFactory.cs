namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonAreNotEqualExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryExpressionFactory<object?, object?, AreNotEqualExpression>(JsonSchemaPropertyNeq, factory)
{
    public static string JsonSchemaPropertyNeq => "$neq";

    protected override AreNotEqualExpression CreateExpressionInstance(IExpression<Task<object?>> leftExpression, IExpression<Task<object?>> rightExpression) => new(leftExpression, rightExpression);
}
