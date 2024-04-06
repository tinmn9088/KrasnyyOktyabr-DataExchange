namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonBinaryBoolExpressionFactory<TOut>(string expressionName, IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryExpressionFactory<bool, bool, TOut>(expressionName, factory) where TOut : IExpression<Task<bool>>
{
}
