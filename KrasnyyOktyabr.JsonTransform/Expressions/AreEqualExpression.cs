using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class AreEqualExpression(IExpression<Task<object?>> leftExpression, IExpression<Task<object?>> rightExpression)
    : AbstractBinaryExpression<object?, object?, bool>(leftExpression, rightExpression)
{
    protected override ValueTask<bool> CalculateAsync(object? left, object? right)
    {
        if (left == right)
        {
            return ValueTask.FromResult(true);
        }

        JToken leftToken = FromObject(left);
        JToken rightToken = FromObject(right);

        return ValueTask.FromResult(JToken.DeepEquals(leftToken, rightToken));
    }

    private static JToken FromObject(object? value)
    {
        return value == null
            ? JValue.CreateNull()
            : JToken.FromObject(value);
    }
}
