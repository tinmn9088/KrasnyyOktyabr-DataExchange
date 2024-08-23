using Newtonsoft.Json.Linq;

using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonConstExpressionFactory : AbstractJsonExpressionFactory<ConstExpression>
{
    public static string JsonSchemaPropertyConst => "$const";

    public JsonConstExpressionFactory()
        : base(@"{
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
            }")
    {
    }

    public override ConstExpression Create(JToken input)
    {
        JToken? value = input[JsonSchemaPropertyConst];
        return new(value);
    }
}
