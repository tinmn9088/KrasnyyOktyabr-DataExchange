namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <summary>
/// Factory for a particular expression.
/// </summary>
public interface IExpressionFactory<TIn, out TOut> : IExpressionMatcher<TIn> where TOut : IExpression<Task>
{
    TOut Create(TIn input);
}
