namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="object?[]"/> or wraps value in an array.
/// </summary>
public sealed class ArrayCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<object?[]>(innerExpression)
{
    public override object?[] Cast(object? innerExpressionTaskResult)
    {
        if (innerExpressionTaskResult is object?[] arrayResult)
        {
            return arrayResult;
        }
        else if (innerExpressionTaskResult is IEnumerable<object?> enumerableResult)
        {
            return enumerableResult.ToArray();
        }
        else
        {
            return [innerExpressionTaskResult];
        }
    }
}
