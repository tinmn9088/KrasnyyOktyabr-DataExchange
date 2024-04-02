using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public static class JsonNumberExpressionFactoriesHelper
{
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task<JsonSchema> BuildJsonSchema(string expressionName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expressionName);

        return await JsonSchema.FromJsonAsync(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                    'type': 'string'
                },
                '" + expressionName + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyLeft + @"': {},
                    '" + JsonSchemaPropertyRight + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyLeft + @"',
                    '" + JsonSchemaPropertyRight + @"'
                  ]
                }
              },
              'required': [
                '" + expressionName + @"'
              ]
            }");
    }

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public static void GetExpressions(
        JToken input,
        string expressionName,
        IJsonAbstractExpressionFactory factory,
        out IExpression<Task<Number>> leftExpression,
        out IExpression<Task<Number>> rightExpression)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentException.ThrowIfNullOrWhiteSpace(expressionName);

        JObject instruction = (JObject)input[expressionName]!;
        JToken leftInstruction = instruction[JsonSchemaPropertyLeft]!;
        JToken rightInstruction = instruction[JsonSchemaPropertyRight]!;

        leftExpression = factory.Create<IExpression<Task<Number>>>(leftInstruction);
        rightExpression = factory.Create<IExpression<Task<Number>>>(rightInstruction);
    }
}
