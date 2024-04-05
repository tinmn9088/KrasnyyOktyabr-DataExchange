using KrasnyyOktyabr.JsonTransform.Numerics;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonBinaryNumberExpressionFactory<TOut> : AbstractJsonExpressionFactory<TOut> where TOut : IExpression<Task<Number>>
{
    private readonly string _expressionName;

    private readonly IJsonAbstractExpressionFactory _factory;

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public AbstractJsonBinaryNumberExpressionFactory(string expressionName, IJsonAbstractExpressionFactory factory)
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
                    '" + JsonSchemaPropertyLeft + @"': {},
                    '" + JsonSchemaPropertyRight + @"': {}
                  },
                  'required': [
                    '" + JsonSchemaPropertyLeft + @"',
                    '" + JsonSchemaPropertyRight + @"'
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
        JToken leftInstruction = instruction[JsonSchemaPropertyLeft]!;
        JToken rightInstruction = instruction[JsonSchemaPropertyRight]!;

        IExpression<Task<Number>> leftExpression = _factory.Create<IExpression<Task<Number>>>(leftInstruction);
        IExpression<Task<Number>> rightExpression = _factory.Create<IExpression<Task<Number>>>(rightInstruction);

        return CreateExpressionInstance(leftExpression, rightExpression);
    }

    protected abstract TOut CreateExpressionInstance(IExpression<Task<Number>> leftExpression, IExpression<Task<Number>> rightExpression);
}
