using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonStringFormatExpressionFactory : AbstractJsonExpressionFactory<StringFormatExpression>
{
    public static string JsonSchemaPropertyStrformat => "$strformat";

    public static string JsonSchemaPropertyArgs => "args";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonStringFormatExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
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
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override StringFormatExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyStrformat]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;
        JArray argInstructions = (JArray)instruction[JsonSchemaPropertyArgs]!; // checked cast, look at JSON Schema

        IExpression<Task<string>> formatExpression = _factory.Create<IExpression<Task<string>>>(valueInstruction);
        List<IExpression<Task<object?>>> argExpressions = argInstructions
                .Select(_factory.Create<IExpression<Task<object?>>>)
                .ToList();

        return new StringFormatExpression(formatExpression, argExpressions);
    }
}
