using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonIfElseExpressionFactory : AbstractJsonExpressionFactory<IfElseExpression>
{
    public static string JsonSchemaPropertyIf => "$while";

    public static string JsonSchemaPropertyThen => "then";

    public static string JsonSchemaPropertyElse => "else";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonIfElseExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyIf + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyCondition + @"': {},
                    '" + JsonSchemaPropertyThen + @"': {},
                    '" + JsonSchemaPropertyElse + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyCondition + @"',
                    '" + JsonSchemaPropertyThen + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyIf + @"'
              ]
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override IfElseExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyIf]!;
        JToken conditionInstruction = instruction[JsonSchemaPropertyCondition]!;
        JToken thenInstruction = instruction[JsonSchemaPropertyThen]!;
        JToken? elseInstruction = instruction[JsonSchemaPropertyElse];

        IExpression<Task<bool>> conditionExpression = _factory.Create<IExpression<Task<bool>>>(conditionInstruction);
        IExpression<Task> thenExpression = _factory.Create<IExpression<Task>>(thenInstruction);

        if (elseInstruction != null)
        {
            IExpression<Task> elseExpression = _factory.Create<IExpression<Task>>(elseInstruction);

            return new(conditionExpression, thenExpression, elseExpression);
        }
        else
        {
            return new(conditionExpression, thenExpression);
        }
    }
}
