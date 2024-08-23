using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonWhileExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<WhileExpression>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertyWhile + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyCondition + @"': {},
                '" + JsonSchemaPropertyInstructions + @"': {}
                },
                'required': [
                '" + JsonSchemaPropertyCondition + @"',
                '" + JsonSchemaPropertyInstructions + @"'
                ]
            }
            },
            'required': [
            '" + JsonSchemaPropertyWhile + @"'
            ]
        }")
{
    public static string JsonSchemaPropertyWhile => "$while";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override WhileExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyWhile]!;
        JToken conditionInstruction = instruction[JsonSchemaPropertyCondition]!;
        JToken instructionsInstruction = instruction[JsonSchemaPropertyInstructions]!;

        IExpression<Task<bool>> conditionExpression = _factory.Create<IExpression<Task<bool>>>(conditionInstruction);
        IExpression<Task> innerExpression = _factory.Create<IExpression<Task>>(instructionsInstruction);

        return new(conditionExpression, innerExpression);
    }
}
