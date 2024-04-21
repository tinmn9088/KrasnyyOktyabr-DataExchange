using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonCursorExpressionFactory : AbstractJsonExpressionFactory<CursorExpression>
{
    public static string JsonSchemaPropertyCur => "$cur";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonCursorExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyCur + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyName + @"': {}
                  }
                }
              },
              'required': [
                '" + JsonSchemaPropertyCur + @"'
              ]
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override CursorExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyCur]!;
        JToken? nameInstruction = instruction[JsonSchemaPropertyName];

        if (nameInstruction != null)
        {
            IExpression<Task<string>> nameExpression = _factory.Create<IExpression<Task<string>>>(nameInstruction);

            return new(nameExpression);
        }
        else
        {
            return new();
        }
    }
}
