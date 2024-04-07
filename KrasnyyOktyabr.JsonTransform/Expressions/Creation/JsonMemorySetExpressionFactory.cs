using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMemorySetExpressionFactory : AbstractJsonExpressionFactory<MemorySetExpression>
{
    public static string JsonSchemaPropertyMSet => "$mset";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonMemorySetExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyMSet + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyName + @"': {},
                    '" + JsonSchemaPropertyValue + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyName + @"',
                    '" + JsonSchemaPropertyValue + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyMSet + @"'
              ]
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override MemorySetExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyMSet]!;
        JToken nameInstruction = instruction[JsonSchemaPropertyName]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;

        IExpression<Task<string>> nameExpression = _factory.Create<IExpression<Task<string>>>(nameInstruction);
        IExpression<Task<object?>> valueExpression = _factory.Create<IExpression<Task<object?>>>(valueInstruction);

        return new MemorySetExpression(nameExpression, valueExpression);
    }
}
