using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions;

/// <summary>
/// Translates inner expression result to <see cref="string"/> or throws <see cref="NullReferenceException"/>.
/// </summary>
public sealed class StringCastExpression(IExpression<Task> innerExpression) : AbstractCastExpression<string>(innerExpression)
{
    /// <summary>
    /// Supports proper serialization of <see cref="Number"/>.
    /// </summary>
    private static readonly JsonSerializer s_jsonSerializer = JsonSerializer.Create(new() { Converters = [new NumberJsonConverter()] });

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public override string Cast(object? innerExpressionTaskResult)
    {
        if (innerExpressionTaskResult is null)
        {
            throw new ArgumentNullException(nameof(innerExpressionTaskResult));
        }

        if (innerExpressionTaskResult is string stringResult)
        {
            return stringResult;
        }

        return JToken.FromObject(innerExpressionTaskResult, s_jsonSerializer).ToString();
    }
}
