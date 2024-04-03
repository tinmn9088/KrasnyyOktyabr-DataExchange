﻿namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractCastExpression<T> : AbstractExpression<Task<T>>
{
    private readonly IExpression<Task> _innerExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractCastExpression(IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(innerExpression);

        _innerExpression = innerExpression;
    }

    public override async Task<T> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        object? innerExpressionTaskResult = await ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken));

        return Cast(innerExpressionTaskResult);
    }

    public abstract T Cast(object? innerExpressionTaskResult);

    public abstract class AbstractCastExpressionException(object? value, Type castType)
        : Exception($"Cannot cast value '{value}' to '{castType.Name}'")
    {
    }

    private static async Task<object?> ExtractTaskResultAsync(Task task)
    {
        await task;

        return task.GetType()
            .GetProperty("Result")
            ?.GetValue(task);
    }
}
