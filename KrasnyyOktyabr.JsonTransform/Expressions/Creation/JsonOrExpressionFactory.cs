
namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonOrExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryBoolExpressionFactory<OrExpression>(JsonSchemaPropertyOr, factory)
{
    public static string JsonSchemaPropertyOr => "$or";

    protected override OrExpression CreateExpressionInstance(IExpression<Task<bool>> leftExpression, IExpression<Task<bool>> rightExpression) => new(leftExpression, rightExpression);
}
