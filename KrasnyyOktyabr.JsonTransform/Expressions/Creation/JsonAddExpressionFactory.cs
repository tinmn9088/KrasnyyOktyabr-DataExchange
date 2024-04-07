using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonAddExpressionFactory : AbstractJsonExpressionFactory<AddExpression>
{
    public static string JsonSchemaPropertyAdd => "$add";

    public static string JsonSchemaPropertyIndex => "index";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonAddExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
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
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override AddExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyAdd]!;
        JToken keyInstruction = instruction[JsonSchemaPropertyKey]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;
        JToken? indexInstruction = instruction[JsonSchemaPropertyIndex];

        IExpression<Task<string>> keyExpression = _factory.Create<IExpression<Task<string>>>(keyInstruction);
        IExpression<Task<object?>> valueExpression = _factory.Create<IExpression<Task<object?>>>(valueInstruction);

        if (indexInstruction != null)
        {
            IExpression<Task<int>> indexExpression = _factory.Create<IExpression<Task<int>>>(indexInstruction);

            return new AddExpression(keyExpression, valueExpression, indexExpression);
        }
        else
        {
            return new AddExpression(keyExpression, valueExpression);
        }
    }
}
