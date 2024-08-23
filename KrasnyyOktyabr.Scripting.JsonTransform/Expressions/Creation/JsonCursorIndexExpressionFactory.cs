using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonCursorIndexExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<CursorIndexExpression>(@"{
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
    public static string JsonSchemaPropertyCurIndex => "$curindex";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override CursorIndexExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyCurIndex]!;
        JToken? nameInstruction = instruction[JsonSchemaPropertyName];

        if (nameInstruction is not null)
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
