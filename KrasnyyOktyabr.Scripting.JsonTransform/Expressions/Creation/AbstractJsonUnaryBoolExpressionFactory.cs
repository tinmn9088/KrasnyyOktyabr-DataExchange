namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonUnaryBoolExpressionFactory<TOut>(string expressionName, IJsonAbstractExpressionFactory factory)
    : AbstractJsonUnaryExpressionFactory<bool, TOut>(expressionName, factory) where TOut : IExpression<Task<bool>>
{
}
