using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonNumberExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMultiplyExpressionFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<MultiplyExpression>
{
    public static string JsonSchemaPropertyMultiply => "$mul";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() => BuildJsonSchema(expressionName: JsonSchemaPropertyMultiply).Result);

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public MultiplyExpression Create(JToken input)
    {
        GetExpressions(
            input,
            expressionName: JsonSchemaPropertyMultiply,
            factory,
            out IExpression<Task<Number>> leftExpression,
            out IExpression<Task<Number>> rightExpression);

        return new MultiplyExpression(leftExpression, rightExpression);
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
