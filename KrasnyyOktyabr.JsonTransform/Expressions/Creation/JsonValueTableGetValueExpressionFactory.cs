using KrasnyyOktyabr.JsonTransform.Structures;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableGetValueExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableGetValueExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyGetValue + @"': {
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
                '" + JsonSchemaPropertyGetValue + @"'
              ]
            }")
{
    public static string JsonSchemaPropertyGetValue => "$getvalue";

    public static string JsonSchemaPropertyTable => "table";

    public static string JsonSchemaPropertyColumn => "column";

    public override ValueTableGetValueExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyGetValue]!;
        JToken valueTableInstruction = instruction[JsonSchemaPropertyTable]!;
        JToken columnInstruction = instruction[JsonSchemaPropertyColumn]!;

        IExpression<Task<IValueTable>> valueTableExpression = factory.Create<IExpression<Task<IValueTable>>>(valueTableInstruction);
        IExpression<Task<string>> columnExpression = factory.Create<IExpression<Task<string>>>(columnInstruction);

        return new(valueTableExpression, columnExpression);
    }
}
