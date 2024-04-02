using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonStringFormatExpressionFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<StringFormatExpression>
{
    public static string JsonSchemaPropertyStrformat => "$strformat";

    public static string JsonSchemaPropertyArgs => "args";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() =>
        JsonSchema.FromJsonAsync(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                    'type': 'string'
                },
                '" + JsonSchemaPropertyStrformat + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyValue + @"': {},
                    '" + JsonSchemaPropertyArgs + @"': {
                      'type': 'array'
                    }
                  },
                  'required': [
                    '" + JsonSchemaPropertyValue + @"',
                    '" + JsonSchemaPropertyArgs + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyStrformat + @"'
              ]
            }").Result);

    /// <exception cref="ArgumentNullException"></exception>
    public StringFormatExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyStrformat]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;
        JArray argInstructions = (JArray)instruction[JsonSchemaPropertyArgs]!; // checked cast, look at JSON Schema

        IExpression<Task<string>> formatExpression = factory.Create<IExpression<Task<string>>>(valueInstruction);
        List<IExpression<Task<object?>>> argExpressions = argInstructions
                .Select(factory.Create<IExpression<Task<object?>>>)
                .ToList();

        return new StringFormatExpression(formatExpression, argExpressions);
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
