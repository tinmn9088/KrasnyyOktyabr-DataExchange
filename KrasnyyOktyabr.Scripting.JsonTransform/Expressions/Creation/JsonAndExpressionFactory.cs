namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonAndExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryBoolExpressionFactory<AndExpression>(JsonSchemaPropertyAnd, factory)
{
    public static string JsonSchemaPropertyAnd => "$and";

    protected override AndExpression CreateExpressionInstance(IExpression<Task<bool>> leftExpression, IExpression<Task<bool>> rightExpression) => new(leftExpression, rightExpression);
}
