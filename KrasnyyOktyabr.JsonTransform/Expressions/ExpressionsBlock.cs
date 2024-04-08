namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Sequence of expressions.
/// </summary>
public sealed class ExpressionsBlock : AbstractExpression<Task<object?[]>>
{
    private readonly IReadOnlyList<IExpression<Task<object?>>> _expressions;

    /// <exception cref="ArgumentNullException"></exception>
    public ExpressionsBlock(IReadOnlyList<IExpression<Task<object?>>> expressions)
    {
        ArgumentNullException.ThrowIfNull(expressions);

        if (expressions.Any(e => e == null))
        {
            throw new ArgumentNullException(nameof(expressions));
        }

        _expressions = expressions;
    }

    protected override async Task<object?[]> InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        object?[] result = new object?[_expressions.Count];

        for (int i = 0; i < _expressions.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            result[i] = await _expressions[i].InterpretAsync(context, cancellationToken).ConfigureAwait(false);
        }

        return result;
    }
}
