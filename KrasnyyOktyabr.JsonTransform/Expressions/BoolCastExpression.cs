namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="bool"/> or translates it to <see cref="string"/> and parses.
/// </summary>
public sealed class BoolCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<bool>(innerExpression)
{
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="BoolCastExpressionException"></exception>
    public override bool Cast(object? innerExpressionTaskResult)
    {
        ArgumentNullException.ThrowIfNull(innerExpressionTaskResult);

        if (innerExpressionTaskResult is bool boolResult)
        {
            return boolResult;
        }
        if (bool.TryParse(innerExpressionTaskResult.ToString(), out bool parseResult))
        {
            return parseResult;
        }
        else
        {
            throw new BoolCastExpressionException(innerExpressionTaskResult);
        }
    }

    public class BoolCastExpressionException : AbstractCastExpressionException
    {
        internal BoolCastExpressionException(object? value) : base(value, typeof(bool))
        {
        }
    }
}
