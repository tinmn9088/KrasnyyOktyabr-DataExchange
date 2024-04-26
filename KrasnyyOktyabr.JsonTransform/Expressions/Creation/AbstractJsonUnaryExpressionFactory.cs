using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <exception cref="ArgumentException"></exception>
/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractJsonUnaryExpressionFactory<TValue, TOut>(string expressionName, IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<TOut>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + expressionName + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyValue + @"': {}
                },
                'required': [
                '" + JsonSchemaPropertyValue + @"'
                ]
            }
            },
            'required': [
            '" + expressionName + @"'
            ]
        }")
    where TOut : IExpression<Task>
{
    private readonly string _expressionName = expressionName ?? throw new ArgumentNullException(nameof(expressionName));

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    public override TOut Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[_expressionName]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;

        IExpression<Task<TValue>> valueExpression = _factory.Create<IExpression<Task<TValue>>>(valueInstruction);

        return CreateExpressionInstance(valueExpression);
    }

    protected abstract TOut CreateExpressionInstance(IExpression<Task<TValue>> valueExpression);
}
