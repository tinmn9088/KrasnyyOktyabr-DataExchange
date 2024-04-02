using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonNumberExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonSumExpressionFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<SumExpression>
{
    public static string JsonSchemaPropertySum => "$sum";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() => BuildJsonSchema(expressionName: JsonSchemaPropertySum).Result);

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SumExpression Create(JToken input)
    {
        GetExpressions(
            input,
            expressionName: JsonSchemaPropertySum,
            factory,
            out IExpression<Task<Number>> leftExpression,
            out IExpression<Task<Number>> rightExpression);

        return new SumExpression(leftExpression, rightExpression);
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
