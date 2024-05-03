using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;
public sealed class JsonRoundExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<RoundExpression>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertyRound + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyValue + @"': {},
                '" + JsonSchemaPropertyDigits + @"': {}
                },
                'required': [
                '" + JsonSchemaPropertyValue + @"'
                ]
            }
            },
            'required': [
            '" + JsonSchemaPropertyRound + @"'
            ]
        }")
{
    public static string JsonSchemaPropertyRound => "$round";

    public static string JsonSchemaPropertyDigits => "digits";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"
    public override RoundExpression Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyRound]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;
        JToken? digitsInstruction = instruction[JsonSchemaPropertyDigits];

        IExpression<Task<Number>> valueExpression = _factory.Create<IExpression<Task<Number>>>(valueInstruction);

        if (digitsInstruction != null)
        {
            IExpression<Task<int>> digitsExpression = _factory.Create<IExpression<Task<int>>>(digitsInstruction);

            return new(valueExpression, digitsExpression);
        }
        else
        {
            return new(valueExpression);
        }
    }
}
