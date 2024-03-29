namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Translates inner expression result to <see cref="string"/> or throws <see cref="NullReferenceException"/>.
/// </summary>
public sealed class StringCastExpression : IExpression<Task<string>>
{
    private readonly IExpression<Task> _inner;

    /// <exception cref="ArgumentNullException"></exception>
    public StringCastExpression(IExpression<Task> inner)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _inner = inner;
    }

    /// <exception cref="NullReferenceException"></exception>
    public async Task<string> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        object innerExpressionTaskResult = await CastExpressionsHelper.ExtractTaskResultAsync(_inner.InterpretAsync(context, cancellationToken)) ?? throw new NullReferenceException();

        if (innerExpressionTaskResult is string stringResult)
        {
            return stringResult;
        }

        return innerExpressionTaskResult.ToString() ?? throw new NullReferenceException();
    }
}
