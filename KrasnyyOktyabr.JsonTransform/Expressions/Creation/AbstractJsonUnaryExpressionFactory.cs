using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonUnaryExpressionFactory<TValue, TOut> : AbstractJsonExpressionFactory<TOut> where TOut : IExpression<Task>
{
    private readonly string _expressionName;

    private readonly IJsonAbstractExpressionFactory _factory;

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public AbstractJsonUnaryExpressionFactory(string expressionName, IJsonAbstractExpressionFactory factory)
        : base(@"{
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
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expressionName);
        ArgumentNullException.ThrowIfNull(factory);

        _expressionName = expressionName;
        _factory = factory;
    }

    public override TOut Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[_expressionName]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;

        IExpression<Task<TValue>> valueExpression = _factory.Create<IExpression<Task<TValue>>>(valueInstruction);

        return CreateExpressionInstance(valueExpression);
    }

    protected abstract TOut CreateExpressionInstance(IExpression<Task<TValue>> valueExpression);
}
