namespace KrasnyyOktyabr.JsonTransform.Expressions;

public static class CastExpressionsHelper
{
    public static async Task<object?> ExtractTaskResultAsync(Task task)
    {
        await task;

        return task.GetType()
            .GetProperty("Result")
            ?.GetValue(task);
    }

    public abstract class AbstractCastExpressionException(object? value, Type castType)
        : Exception($"Cannot cast value '{value}' to '{castType.Name}'")
    {
    }
}
