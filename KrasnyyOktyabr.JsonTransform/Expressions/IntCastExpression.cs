using System.Globalization;

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
        ArgumentNullException.ThrowIfNull(innerExpressionTaskResult);

        if (innerExpressionTaskResult is int intResult)
        {
            return intResult;
        }
        if (int.TryParse(innerExpressionTaskResult?.ToString(), CultureInfo.InvariantCulture, out int parseResult))
        {
            return parseResult;
        }
        else
        {
            throw new IntCastExpressionException(innerExpressionTaskResult);
        }
    }

    public class IntCastExpressionException : AbstractCastExpressionException
    {
        internal IntCastExpressionException(object? value) : base(value, typeof(int))
        {
        }
    }
}
