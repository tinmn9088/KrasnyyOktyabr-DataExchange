using KrasnyyOktyabr.JsonTransform.Structures;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonValueTableCollapseExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ValueTableCollapseExpression>(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyCollapse + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyTable + @"': {},
                    '" + JsonSchemaPropertyColumnsToGroup + @"': {},
                    '" + JsonSchemaPropertyColumnsToSum + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyTable + @"',
                    '" + JsonSchemaPropertyColumnsToGroup + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyCollapse + @"'
              ]
            }")
{
    public static string JsonSchemaPropertyCollapse => "$collapse";

    public static string JsonSchemaPropertyTable => "table";

    public static string JsonSchemaPropertyColumnsToGroup => "group";

    public static string JsonSchemaPropertyColumnsToSum => "sum";

    public override ValueTableCollapseExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyCollapse]!;
        JToken valueTableInstruction = instruction[JsonSchemaPropertyTable]!;
        JToken columnsToGroupStringInstruction = instruction[JsonSchemaPropertyColumnsToGroup]!;
        JToken? columnsToSumStringInstruction = instruction[JsonSchemaPropertyColumnsToSum];

        IExpression<Task<IValueTable>> valueTableExpression = factory.Create<IExpression<Task<IValueTable>>>(valueTableInstruction);
        IExpression<Task<string>> columnsToGroupStringExpression = factory.Create<IExpression<Task<string>>>(columnsToGroupStringInstruction);

        if (columnsToSumStringInstruction is not null)
        {
            IExpression<Task<string>> columnsToSumStringExpression = factory.Create<IExpression<Task<string>>>(columnsToSumStringInstruction);

            return new(valueTableExpression, columnsToGroupStringExpression, columnsToSumStringExpression);
        }
        else
        {
            return new(valueTableExpression, columnsToGroupStringExpression);
        }
    }
}
