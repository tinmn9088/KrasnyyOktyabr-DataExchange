using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonForeachExpressionFactory : AbstractJsonExpressionFactory<ForeachExpression>
{
    public static string JsonSchemaPropertyForeach => "$foreach";

    public static string JsonSchemaPropertyItems => "items";


    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonForeachExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'object',
              'additionalProperties': false,
              'properties': {
                '" + JsonSchemaPropertyComment + @"': {
                  'type': 'string'
                },
                '" + JsonSchemaPropertyForeach + @"': {
                  'type': 'object',
                  'additionalProperties': false,
                  'properties': {
                    '" + JsonSchemaPropertyItems + @"': {},
                    '" + JsonSchemaPropertyInstructions + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyItems + @"',
                    '" + JsonSchemaPropertyInstructions + @"'
                  ]
                }
              },
              'required': [
                '" + JsonSchemaPropertyForeach + @"'
              ]
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override ForeachExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyForeach]!;
        JToken itemsInstruction = instruction[JsonSchemaPropertyItems]!;
        JToken instructionsInstruction = instruction[JsonSchemaPropertyInstructions]!;

        IExpression<Task<object?[]>> itemsExpression = _factory.Create<IExpression<Task<object?[]>>>(itemsInstruction);
        IExpression<Task> innerExpression = _factory.Create<IExpression<Task>>(instructionsInstruction);

        return new ForeachExpression(itemsExpression, innerExpression);
    }
}
