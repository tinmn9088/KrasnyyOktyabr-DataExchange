using System.Globalization;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="double"/> or translates it to <see cref="string"/> and parses.
/// </summary>
public sealed class DoubleCastExpression : IExpression<Task<double>>
{
    private readonly IExpression<Task> _innerExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public DoubleCastExpression(IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(innerExpression);

        _innerExpression = innerExpression;
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="DoubleCastExpressionException"></exception>
    public async Task<double> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        object innerExpressionTaskResult = await CastExpressionsHelper.ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken)) ?? throw new NullReferenceException();

        if (innerExpressionTaskResult is double doubleResult)
        {
            return doubleResult;
        }
        if (double.TryParse(innerExpressionTaskResult?.ToString(), CultureInfo.InvariantCulture, out double parseResult))
        {
            return parseResult;
        }
        else
        {
            throw new DoubleCastExpressionException(innerExpressionTaskResult);
        }
    }

    public class DoubleCastExpressionException : CastExpressionsHelper.AbstractCastExpressionException
    {
        internal DoubleCastExpressionException(object? value) : base(value, typeof(double))
        {
        }
    }
}
