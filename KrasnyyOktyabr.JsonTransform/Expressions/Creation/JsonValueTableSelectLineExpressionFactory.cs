using KrasnyyOktyabr.JsonTransform.Structures;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableSelectLineExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableSelectLineExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertySelectLine + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyTable + @"': {},
                    '" + JsonSchemaPropertyIndex + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyTable + @"',
                    '" + JsonSchemaPropertyIndex + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertySelectLine + @"'
              ]
            }")
{
    public static string JsonSchemaPropertySelectLine => "$selectline";

    public static string JsonSchemaPropertyTable => "table";

    public static string JsonSchemaPropertyIndex => "index";

    public override ValueTableSelectLineExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertySelectLine]!;
        JToken valueTableInstruction = instruction[JsonSchemaPropertyTable]!;
        JToken indexInstruction = instruction[JsonSchemaPropertyIndex]!;

        IExpression<Task<IValueTable>> valueTableExpression = factory.Create<IExpression<Task<IValueTable>>>(valueTableInstruction);
        IExpression<Task<long>> indexExpression = factory.Create<IExpression<Task<long>>>(indexInstruction);

        return new(valueTableExpression, indexExpression);
    }
}
