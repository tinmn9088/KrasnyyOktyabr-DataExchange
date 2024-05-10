namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractCastExpression<T>(IExpression<Task> innerExpression) : AbstractExpression<Task<T>>
{
    private readonly IExpression<Task> _innerExpression = innerExpression ?? throw new ArgumentNullException(nameof(innerExpression));

    public override async Task<T> InterpretAsync(IContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            object? innerExpressionTaskResult = await ExtractTaskResultAsync(_innerExpression.InterpretAsync(context, cancellationToken)).ConfigureAwait(false);

            return Cast(innerExpressionTaskResult);
        }
        catch (InterpretException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InterpretException(ex.Message, Mark);
        }
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
