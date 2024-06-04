using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMapExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<MapExpression>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertyMap + @"': {
                'type': 'object'
            }
            },
            'required': [
            '" + JsonSchemaPropertyMap + @"'
            ]
        }")
{
    public static string JsonSchemaPropertyMap => "$map";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override MapExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyMap]!;

        Dictionary<string, IExpression<Task<object?>>> keysAndExpressions = [];

        foreach (KeyValuePair<string, JToken?> keyAndInstruction in instruction)
        {
            keysAndExpressions.Add(keyAndInstruction.Key, _factory.Create<IExpression<Task<object?>>>(keyAndInstruction.Value!));
        }

        return new(keysAndExpressions);
    }
}
