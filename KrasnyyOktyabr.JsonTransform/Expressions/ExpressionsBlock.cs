namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Sequence of expressions.
/// </summary>
public sealed class ExpressionsBlock : AbstractExpression<Task>
{
    private readonly IReadOnlyList<IExpression<Task>> _expressions;

    /// <exception cref="ArgumentNullException"></exception>
    public ExpressionsBlock(IReadOnlyList<IExpression<Task>> expressions)
    {
        _expressions = expressions ?? throw new ArgumentNullException(nameof(expressions));

        if (expressions.Any(e => e == null))
        {
            throw new ArgumentNullException(nameof(expressions));
        }
    }

    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        for (int i = 0; i < _expressions.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _expressions[i].InterpretAsync(context, cancellationToken).ConfigureAwait(false);
        }
    }
}
