using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonCursorExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<CursorExpression>(@"{
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
    public static string JsonSchemaPropertyCur => "$cur";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override CursorExpression Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

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
