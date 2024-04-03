namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractCastExpression<TOut> : IExpression<Task<TOut>>
{
    private readonly IExpression<Task> _innerExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractCastExpression(IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(innerExpression);

        _innerExpression = innerExpression;
    }

    public async Task<TOut> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        object? innerExpressionTaskResult = await ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken));

        return Cast(innerExpressionTaskResult);
    }

    public abstract TOut Cast(object? innerExpressionTaskResult);

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
