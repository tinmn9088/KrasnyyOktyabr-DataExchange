using KrasnyyOktyabr.JsonTransform.Structures;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableCountExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableCountExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyTableSize + @"': {
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
                '" + JsonSchemaPropertyTableSize + @"'
              ]
            }")
{
    public static string JsonSchemaPropertyTableSize => "$tablesize";

    public static string JsonSchemaPropertyTable => "table";

    public override ValueTableCountExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyTableSize]!;
        JToken valueTableInstruction = instruction[JsonSchemaPropertyTable]!;

        IExpression<Task<IValueTable>> valueTableExpression = factory.Create<IExpression<Task<IValueTable>>>(valueTableInstruction);

        return new(valueTableExpression);
    }
}
