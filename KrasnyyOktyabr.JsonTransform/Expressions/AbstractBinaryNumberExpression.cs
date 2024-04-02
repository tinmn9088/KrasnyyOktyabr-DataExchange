﻿using KrasnyyOktyabr.JsonTransform.Numerics;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Abstract expression which consumes 2 <see cref="Number"/>'s and produces <see cref="Number"/>.
/// </summary>
public abstract class AbstractBinaryNumberExpression : IExpression<Task<Number>>
{
    protected readonly IExpression<Task<Number>> _leftExpression;

    protected readonly IExpression<Task<Number>> _rightExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractBinaryNumberExpression(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(leftExpression);
        ArgumentNullException.ThrowIfNull(rightExpression);

        _leftExpression = leftExpression;
        _rightExpression = rightExpression;
    }

    public async Task<Number> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        Number left = await _leftExpression.InterpretAsync(context, cancellationToken);
        Number right = await _rightExpression.InterpretAsync(context, cancellationToken);

        return Calculate(left, right);
    }

    protected abstract Number Calculate(Number left, Number right);
}
