namespace KrasnyyOktyabr.JsonTransform.Expressions;

public abstract class AbstractCastExpression<T> : AbstractExpression<Task<T>>
{
    private readonly IExpression<Task> _innerExpression;

    /// <exception cref="ArgumentNullException"></exception>
    public AbstractCastExpression(IExpression<Task> innerExpression)
    {
        ArgumentNullException.ThrowIfNull(innerExpression);

        _innerExpression = innerExpression;
    }

    protected override async Task<T> InnerInterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        object? innerExpressionTaskResult = await ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken)).ConfigureAwait(false);

        return Cast(innerExpressionTaskResult);
    }

    public abstract T Cast(object? innerExpressionTaskResult);

    public abstract class AbstractCastExpressionException(object? value, Type castType, string? mark)
        : InterpretException($"Cannot cast value '{value}' to '{castType.Name}'", mark)
    {
    }

    private static async Task<object?> ExtractTaskResultAsync(Task task)
    {
        await task.ConfigureAwait(false);

        return task.GetType()
            .GetProperty("Result")
            ?.GetValue(task);
    }
}
