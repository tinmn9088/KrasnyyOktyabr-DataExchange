using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonBinaryNumberExpressionFactory<TOut>(string expressionName, IJsonAbstractExpressionFactory factory)
    : AbstractJsonBinaryExpressionFactory<Number, Number, TOut>(expressionName, factory) where TOut : IExpression<Task<Number>>
{
}
