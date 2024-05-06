using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

public class AreNotEqualExpression(IExpression<Task<object?>> leftExpression, IExpression<Task<object?>> rightExpression)
    : AbstractBinaryExpression<object?, object?, bool>(leftExpression, rightExpression)
{
    /// <summary>
    /// Supports proper serialization of <see cref="Number"/>.
    /// </summary>
    private static readonly JsonSerializer s_jsonSerializer = JsonSerializer.Create(new() { Converters = [new NumberJsonConverter()] });

    protected override async ValueTask<bool> CalculateAsync(Func<Task<object?>> getLeft, Func<Task<object?>> getRight)
    {
        object? left = await getLeft().ConfigureAwait(false);
        object? right = await getRight().ConfigureAwait(false);

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
            : JToken.FromObject(value, s_jsonSerializer);
    }
}
