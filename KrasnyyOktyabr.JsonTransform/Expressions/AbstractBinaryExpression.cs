﻿namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractBinaryExpression<TLeft, TRight, TResult> : AbstractExpression<Task<TResult>>
{
    private readonly IExpression<Task<TLeft>> _leftExpression;

    private readonly IExpression<Task<TRight>> _rightExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractBinaryExpression(IExpression<Task<TLeft>> leftExpression, IExpression<Task<TRight>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(leftExpression);
        ArgumentNullException.ThrowIfNull(rightExpression);

        _leftExpression = leftExpression;
        _rightExpression = rightExpression;
    }

    protected override async Task<TResult> InnerInterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        Task<TLeft> getLeft() => _leftExpression.InterpretAsync(context, cancellationToken);
        Task<TRight> getRight() => _rightExpression.InterpretAsync(context, cancellationToken);

        return await CalculateAsync(getLeft, getRight);
    }

    protected abstract ValueTask<TResult> CalculateAsync(Func<Task<TLeft>> getLeft, Func<Task<TRight>> getRight);
}

public abstract class AbstractBinaryExpression<T>(IExpression<Task<T>> leftExpression, IExpression<Task<T>> rightExpression)
    : AbstractBinaryExpression<T, T, T>(leftExpression, rightExpression)
{
}