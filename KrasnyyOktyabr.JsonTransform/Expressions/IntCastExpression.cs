namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="int"/> or translates it to <see cref="string"/> and parses.
/// </summary>
public sealed class IntCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<int>(innerExpression)
{
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="IntCastExpressionException"></exception>
    public override int Cast(object? innerExpressionTaskResult)
    {
        if (innerExpressionTaskResult == null)
        {
            throw new ArgumentNullException(nameof(innerExpressionTaskResult));
        }

        if (innerExpressionTaskResult is int intResult)
        {
            return intResult;
        }
        else if (int.TryParse(innerExpressionTaskResult?.ToString(), out int parseResult))
        {
            return parseResult;
        }
        else
        {
            throw new IntCastExpressionException(innerExpressionTaskResult, Mark);
        }
    }

    public class IntCastExpressionException : AbstractCastExpressionException
    {
        internal IntCastExpressionException(object? value, string? mark) : base(value, typeof(int), mark)
        {
        }
    }
}
