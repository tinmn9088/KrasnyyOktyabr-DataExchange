namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Casts inner expression result to <see cref="bool"/> or translates it to <see cref="string"/> and parses.
/// </summary>
public sealed class BoolCastExpression : IExpression<Task<bool>>
{
    private readonly IExpression<Task> _innerExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public BoolCastExpression(IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(innerExpression);

        _innerExpression = innerExpression;
    }

    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="BoolCastExpressionException"></exception>
    public async Task<bool> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        object innerExpressionTaskResult = await CastExpressionsHelper.ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken)) ?? throw new NullReferenceException();

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

    public class BoolCastExpressionException : CastExpressionsHelper.AbstractCastExpressionException
    {
        internal BoolCastExpressionException(object? value) : base(value, typeof(bool))
        {
        }
    }
}
