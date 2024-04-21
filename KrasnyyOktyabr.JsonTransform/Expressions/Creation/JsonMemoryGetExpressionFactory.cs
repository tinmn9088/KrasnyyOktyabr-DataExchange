using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMemoryGetExpressionFactory : AbstractJsonExpressionFactory<MemoryGetExpression>
{
    public static string JsonSchemaPropertyMGet => "$mget";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonMemoryGetExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyMGet + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyName + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyName + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyMGet + @"'
              ]
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override MemoryGetExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyMGet]!;
        JToken nameInstruction = instruction[JsonSchemaPropertyName]!;

        IExpression<Task<string>> nameExpression = _factory.Create<IExpression<Task<string>>>(nameInstruction);

        return new(nameExpression);
    }
}
