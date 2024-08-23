using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonAddExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<AddExpression>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertyAdd + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyKey + @"': {},
                '" + JsonSchemaPropertyValue + @"': {},
                '" + JsonSchemaPropertyIndex + @"': {}
                },
                'required': [
                '" + JsonSchemaPropertyKey + @"',
                '" + JsonSchemaPropertyValue + @"'
                ]
            }
            },
            'required': [
            '" + JsonSchemaPropertyAdd + @"'
            ]
        }")
{
    public static string JsonSchemaPropertyAdd => "$add";

    public static string JsonSchemaPropertyIndex => "index";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override AddExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyAdd]!;
        JToken keyInstruction = instruction[JsonSchemaPropertyKey]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;
        JToken? indexInstruction = instruction[JsonSchemaPropertyIndex];

        IExpression<Task<string>> keyExpression = _factory.Create<IExpression<Task<string>>>(keyInstruction);
        IExpression<Task<object?>> valueExpression = _factory.Create<IExpression<Task<object?>>>(valueInstruction);

        if (indexInstruction is not null)
        {
            IExpression<Task<long>> indexExpression = _factory.Create<IExpression<Task<long>>>(indexInstruction);

            return new(keyExpression, valueExpression, indexExpression);
        }
        else
        {
            return new(keyExpression, valueExpression);
        }
    }
}
