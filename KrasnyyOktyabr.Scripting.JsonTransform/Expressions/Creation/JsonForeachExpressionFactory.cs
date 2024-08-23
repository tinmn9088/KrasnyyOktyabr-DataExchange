using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonForeachExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ForeachExpression>(@"{
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
                '" + JsonSchemaPropertyName + @"': {
                    'type': 'string'
                },
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
    public static string JsonSchemaPropertyForeach => "$foreach";

    public static string JsonSchemaPropertyItems => "items";


    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override ForeachExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyForeach]!;
        JToken itemsInstruction = instruction[JsonSchemaPropertyItems]!;
        JToken instructionsInstruction = instruction[JsonSchemaPropertyInstructions]!;
        string? name = instruction[JsonSchemaPropertyName]?.Value<string>();

        IExpression<Task<object?[]>> itemsExpression = _factory.Create<IExpression<Task<object?[]>>>(itemsInstruction);
        IExpression<Task> innerExpression = _factory.Create<IExpression<Task>>(instructionsInstruction);

        if (name is not null)
        {
            return new(itemsExpression, innerExpression, name);
        }
        else
        {
            return new(itemsExpression, innerExpression);
        }
    }
}
