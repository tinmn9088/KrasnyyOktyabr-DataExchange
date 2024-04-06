using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonUnaryNumberExpressionFactory<TOut>(string expressionName, IJsonAbstractExpressionFactory factory)
    : AbstractJsonUnaryExpressionFactory<Number, TOut>(expressionName, factory) where TOut : IExpression<Task<Number>>
{
}
