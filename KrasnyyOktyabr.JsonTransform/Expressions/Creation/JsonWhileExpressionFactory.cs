using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonWhileExpressionFactory : AbstractJsonExpressionFactory<WhileExpression>
{
    public static string JsonSchemaPropertyWhile => "$while";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonWhileExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
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
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override WhileExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyWhile]!;
        JToken conditionInstruction = instruction[JsonSchemaPropertyCondition]!;
        JToken instructionsInstruction = instruction[JsonSchemaPropertyInstructions]!;

        IExpression<Task<bool>> conditionExpression = _factory.Create<IExpression<Task<bool>>>(conditionInstruction);
        IExpression<Task> innerExpression = _factory.Create<IExpression<Task>>(instructionsInstruction);

        return new(conditionExpression, innerExpression);
    }
}
