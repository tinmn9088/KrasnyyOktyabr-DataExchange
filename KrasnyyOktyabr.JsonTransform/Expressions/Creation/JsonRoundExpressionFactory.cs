using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;
public sealed class JsonRoundExpressionFactory : AbstractJsonExpressionFactory<RoundExpression>
{
    public static string JsonSchemaPropertyRound => "$round";

    public static string JsonSchemaPropertyDigits => "digits";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonRoundExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
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
                    '" + JsonSchemaPropertyDigits + @"': {
                      type: 'integer'
                    }
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
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"
    public override RoundExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyRound]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;
        int? digits = instruction[JsonSchemaPropertyDigits]?.Value<int>();

        IExpression<Task<Number>> valueExpression = _factory.Create<IExpression<Task<Number>>>(valueInstruction);

        if (digits != null)
        {
            return new(valueExpression, digits.Value);
        }
        else
        {
            return new(valueExpression);
        }
    }
}
