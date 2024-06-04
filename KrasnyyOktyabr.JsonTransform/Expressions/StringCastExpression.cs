namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Translates inner expression result to <see cref="string"/> or throws <see cref="NullReferenceException"/>.
/// </summary>
public sealed class StringCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<string>(innerExpression)
{
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public override string Cast(object? innerExpressionTaskResult)
    {
        if (innerExpressionTaskResult is null)
        {
            throw new ArgumentNullException(nameof(innerExpressionTaskResult));
        }

        if (innerExpressionTaskResult is string stringResult)
        {
            return stringResult;
        }

        return innerExpressionTaskResult.ToString() ?? throw new NullReferenceException();
    }
}
