using KrasnyyOktyabr.JsonTransform.Structures;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableAddColumnExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableAddColumnExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyAddColumn + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyTable + @"': {},
                    '" + JsonSchemaPropertyColumn + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyTable + @"',
                    '" + JsonSchemaPropertyColumn + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyAddColumn + @"'
              ]
            }")
{
    public static string JsonSchemaPropertyAddColumn => "$addcolumn";

    public static string JsonSchemaPropertyTable => "table";

    public static string JsonSchemaPropertyColumn => "column";

    public override ValueTableAddColumnExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyAddColumn]!;
        JToken valueTableInstruction = instruction[JsonSchemaPropertyTable]!;
        JToken columnInstruction = instruction[JsonSchemaPropertyColumn]!;

        IExpression<Task<IValueTable>> valueTableExpression = factory.Create<IExpression<Task<IValueTable>>>(valueTableInstruction);
        IExpression<Task<string>> columnExpression = factory.Create<IExpression<Task<string>>>(columnInstruction);

        return new(valueTableExpression, columnExpression);
    }
}
