using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonMapExpressionFactory : AbstractJsonExpressionFactory<MapExpression>
{
    public static string JsonSchemaPropertyMap => "$map";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonMapExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
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
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override MapExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);
        JObject instruction = (JObject)input[JsonSchemaPropertyMap]!;

        Dictionary<string, IExpression<Task<object?>>> keysAndExpressions = [];

        foreach (KeyValuePair<string, JToken?> keyAndInstruction in instruction)
        {
            keysAndExpressions.Add(keyAndInstruction.Key, _factory.Create<IExpression<Task<object?>>>(keyAndInstruction.Value!));
        }

        return new MapExpression(keysAndExpressions);
    }
}
