using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonCursorIndexExpressionFactory : AbstractJsonExpressionFactory<CursorIndexExpression>
{
    public static string JsonSchemaPropertyCurIndex => "$curindex";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonCursorIndexExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyCurIndex + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyName + @"': {}
                  }
                }
              },
              'required': [
                '" + JsonSchemaPropertyCurIndex + @"'
              ]
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override CursorIndexExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyCurIndex]!;
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
