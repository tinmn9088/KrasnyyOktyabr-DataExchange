using System.Globalization;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="decimal"/> or translates it to <see cref="string"/> and parses.
/// </summary>
public sealed class DoubleCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<decimal>(innerExpression)
{
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="DoubleCastExpressionException"></exception>
    public override decimal Cast(object? innerExpressionTaskResult)
    {
        if (innerExpressionTaskResult == null)
        {
            throw new ArgumentNullException(nameof(innerExpressionTaskResult));
        }

        if (innerExpressionTaskResult is decimal doubleResult)
        {
            return doubleResult;
        }
        else if (decimal.TryParse(
            innerExpressionTaskResult?.ToString(),
            out decimal parse1Result))
        {
            return parse1Result;
        }
        else if (decimal.TryParse(
            innerExpressionTaskResult?.ToString(),
            style: NumberStyles.Any,
            provider: CultureInfo.InvariantCulture,
            out decimal parse2Result))
        {
            return parse2Result;
        }
        else
        {
            throw new DoubleCastExpressionException(innerExpressionTaskResult, Mark);
        }
    }

    public class DoubleCastExpressionException : AbstractCastExpressionException
    {
        internal DoubleCastExpressionException(object? value, string? mark) : base(value, typeof(double), mark)
        {
        }
    }
}
