using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonIfElseExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<IfElseExpression>(@"{
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
    public static string JsonSchemaPropertyIf => "$while";

    public static string JsonSchemaPropertyThen => "then";

    public static string JsonSchemaPropertyElse => "else";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override IfElseExpression Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

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
