using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonNumberExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonSubstractExpressionFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<SubstractExpression>
{
    public static string JsonSchemaPropertySubstract => "$subtract";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() => BuildJsonSchema(expressionName: JsonSchemaPropertySubstract).Result);

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public SubstractExpression Create(JToken input)
    {
        GetExpressions(
            input,
            expressionName: JsonSchemaPropertySubstract,
            factory,
            out IExpression<Task<Number>> leftExpression,
            out IExpression<Task<Number>> rightExpression);

        return new SubstractExpression(leftExpression, rightExpression);
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
