using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonStringFormatExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<StringFormatExpression>(@"{
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
            }")
{
    public static string JsonSchemaPropertyStrformat => "$strformat";

    public static string JsonSchemaPropertyArgs => "args";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override StringFormatExpression Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyStrformat]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;
        JArray argInstructions = (JArray)instruction[JsonSchemaPropertyArgs]!; // checked cast, look at JSON Schema

        IExpression<Task<string>> formatExpression = _factory.Create<IExpression<Task<string>>>(valueInstruction);
        List<IExpression<Task<object?>>> argExpressions = argInstructions
                .Select(_factory.Create<IExpression<Task<object?>>>)
                .ToList();

        return new(formatExpression, argExpressions);
    }
}
