using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMemoryGetExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<MemoryGetExpression>(@"{
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
    public static string JsonSchemaPropertyMGet => "$mget";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override MemoryGetExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyMGet]!;
        JToken nameInstruction = instruction[JsonSchemaPropertyName]!;

        IExpression<Task<string>> nameExpression = _factory.Create<IExpression<Task<string>>>(nameInstruction);

        return new(nameExpression);
    }
}
