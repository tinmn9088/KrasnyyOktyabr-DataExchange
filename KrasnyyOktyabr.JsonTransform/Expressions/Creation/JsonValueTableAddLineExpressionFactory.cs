using KrasnyyOktyabr.JsonTransform.Structures;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableAddLineExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableAddLineExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyAddLine + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyTable + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyTable + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyAddLine + @"'
              ]
            }")
{
    public static string JsonSchemaPropertyAddLine => "$addline";

    public static string JsonSchemaPropertyTable => "table";

    public override ValueTableAddLineExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyAddLine]!;
        JToken valueTableInstruction = instruction[JsonSchemaPropertyTable]!;

        IExpression<Task<IValueTable>> valueTableExpression = factory.Create<IExpression<Task<IValueTable>>>(valueTableInstruction);

        return new(valueTableExpression);
    }
}
