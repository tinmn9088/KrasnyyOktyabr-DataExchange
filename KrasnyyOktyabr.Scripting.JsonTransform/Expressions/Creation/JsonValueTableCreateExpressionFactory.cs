using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableCreateExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableCreateExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyCreateTable + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyColumns + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyColumns + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyCreateTable + @"'
              ]
            }")
{
    public static string JsonSchemaPropertyCreateTable => "$createtable";

    public static string JsonSchemaPropertyColumns => "columns";

    public override ValueTableCreateExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyCreateTable]!;
        JToken columnsStringInstruction = instruction[JsonSchemaPropertyColumns]!;

        IExpression<Task<string>> columnsStringExpression = factory.Create<IExpression<Task<string>>>(columnsStringInstruction);

        return new(columnsStringExpression);
    }
}
