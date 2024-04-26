namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractCastExpression<T>(IExpression<Task> innerExpression) : AbstractExpression<Task<T>>
{
    private readonly IExpression<Task> _innerExpression = innerExpression ?? throw new ArgumentNullException(nameof(innerExpression));

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
