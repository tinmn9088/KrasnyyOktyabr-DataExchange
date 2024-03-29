using System.Globalization;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="int"/> or translates it to <see cref="string"/> and parses.
/// </summary>
public sealed class IntCastExpression : IExpression<Task<int>>
{
    private readonly IExpression<Task> _innerExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public IntCastExpression(IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(innerExpression);

        _innerExpression = innerExpression;
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="IntCastExpressionException"></exception>
    public async Task<int> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        object innerExpressionTaskResult = await CastExpressionsHelper.ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken)) ?? throw new NullReferenceException();

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

    public class IntCastExpressionException : CastExpressionsHelper.AbstractCastExpressionException
    {
        internal IntCastExpressionException(object? value) : base(value, typeof(int))
        {
        }
    }
}
