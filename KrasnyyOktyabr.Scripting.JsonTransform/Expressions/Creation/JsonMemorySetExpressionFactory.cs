using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMemorySetExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<MemorySetExpression>(@"{
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
    public static string JsonSchemaPropertyMSet => "$mset";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override MemorySetExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyMSet]!;
        JToken nameInstruction = instruction[JsonSchemaPropertyName]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;

        IExpression<Task<string>> nameExpression = _factory.Create<IExpression<Task<string>>>(nameInstruction);
        IExpression<Task<object?>> valueExpression = _factory.Create<IExpression<Task<object?>>>(valueInstruction);

        return new(nameExpression, valueExpression);
    }
}
