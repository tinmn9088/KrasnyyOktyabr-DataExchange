using Newtonsoft.Json.Linq;
using NJsonSchema;

using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonConstExpressionFactory : IJsonExpressionFactory<ConstExpression>
{
    public static string JsonSchemaPropertyConst => "$const";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() =>
        JsonSchema.FromJsonAsync(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyConst + @"': {}                
              },
              'required': [
                '" + JsonSchemaPropertyConst + @"'
              ]
            }").Result);

    public ConstExpression Create(JToken input)
    {
        JToken? value = input[JsonSchemaPropertyConst];
        return new ConstExpression(value);
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
