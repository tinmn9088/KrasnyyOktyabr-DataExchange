﻿namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Sequence of expressions.
/// </summary>
public sealed class ExpressionsBlock : IExpression<Task>
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

    public async Task InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        foreach (IExpression<Task> expression in _expressions)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await expression.InterpretAsync(context, cancellationToken);
        }
    }
}