using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonSumExpressionFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<SumExpression>
{
    public static string JsonSchemaPropertySum => "$sum";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() =>
        JsonSchema.FromJsonAsync(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                    'type': 'string'
                },
                '" + JsonSchemaPropertySum + @"': {
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
                '" + JsonSchemaPropertySum + @"'
              ]
            }").Result);

    /// <exception cref="ArgumentNullException"></exception>
    public SumExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertySum]!;
        JToken leftInstruction = instruction[JsonSchemaPropertyLeft]!;
        JToken rightInstruction = instruction[JsonSchemaPropertyRight]!;

        IExpression<Task<Number>> leftExpression = factory.Create<IExpression<Task<Number>>>(leftInstruction);
        IExpression<Task<Number>> rightExpression = factory.Create<IExpression<Task<Number>>>(rightInstruction);

        return new SumExpression(leftExpression, rightExpression);
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
