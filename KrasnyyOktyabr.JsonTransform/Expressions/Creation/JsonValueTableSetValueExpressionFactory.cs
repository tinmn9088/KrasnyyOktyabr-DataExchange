using KrasnyyOktyabr.JsonTransform.Structures;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableSetValueExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableSetValueExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertySetValue + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyTable + @"': {},
                    '" + JsonSchemaPropertyColumn + @"': {},
                    '" + JsonSchemaPropertyValue + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyTable + @"',
                    '" + JsonSchemaPropertyColumn + @"',
                    '" + JsonSchemaPropertyValue + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertySetValue + @"'
              ]
            }")
{
    public static string JsonSchemaPropertySetValue => "$setvalue";

    public static string JsonSchemaPropertyTable => "table";

    public static string JsonSchemaPropertyColumn => "column";

    public override ValueTableSetValueExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertySetValue]!;
        JToken valueTableInstruction = instruction[JsonSchemaPropertyTable]!;
        JToken columnInstruction = instruction[JsonSchemaPropertyColumn]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;

        IExpression<Task<IValueTable>> valueTableExpression = factory.Create<IExpression<Task<IValueTable>>>(valueTableInstruction);
        IExpression<Task<string>> columnExpression = factory.Create<IExpression<Task<string>>>(columnInstruction);
        IExpression<Task<object?>> valueExpression = factory.Create<IExpression<Task<object?>>>(valueInstruction);

        return new(valueTableExpression, columnExpression, valueExpression);
    }
}

