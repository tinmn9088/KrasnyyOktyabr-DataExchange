using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonNumberExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonDivideExpressionFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<DivideExpression>
{
    public static string JsonSchemaPropertyDiv => "$div";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() => BuildJsonSchema(expressionName: JsonSchemaPropertyDiv).Result);

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public DivideExpression Create(JToken input)
    {
        GetExpressions(
            input,
            expressionName: JsonSchemaPropertyDiv,
            factory,
            out IExpression<Task<Number>> leftExpression,
            out IExpression<Task<Number>> rightExpression);

        return new DivideExpression(leftExpression, rightExpression);
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
