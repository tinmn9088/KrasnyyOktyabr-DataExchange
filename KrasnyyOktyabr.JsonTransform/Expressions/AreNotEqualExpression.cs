using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class AreNotEqualExpression(IExpression<Task<object?>> leftExpression, IExpression<Task<object?>> rightExpression)
    : AbstractBinaryExpression<object?, object?, bool>(leftExpression, rightExpression)
{
    protected override async ValueTask<bool> CalculateAsync(Func<Task<object?>> getLeft, Func<Task<object?>> getRight)
    {
        object? left = await getLeft();
        object? right = await getRight();

        if (left == right)
        {
            return false;
        }

        JToken leftToken = FromObject(left);
        JToken rightToken = FromObject(right);

        return !JToken.DeepEquals(leftToken, rightToken);
    }

    private static JToken FromObject(object? value)
    {
        return value == null
            ? JValue.CreateNull()
            : JToken.FromObject(value);
    }
}
