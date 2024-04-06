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
        ArgumentNullException.ThrowIfNull(expressions);

        if (expressions.Any(e => e == null))
        {
            throw new ArgumentNullException(nameof(expressions));
        }

        _expressions = expressions;
    }

    protected override async Task InnerInterpretAsync(IContext context, CancellationToken cancellationToken)
    {
        foreach (IExpression<Task> expression in _expressions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await expression.InterpretAsync(context, cancellationToken);
        }
    }
}
