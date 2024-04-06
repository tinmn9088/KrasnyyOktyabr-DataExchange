using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonBinaryExpressionFactory<TLeft, TRight, TOut> : AbstractJsonExpressionFactory<TOut> where TOut : IExpression<Task>
{
    private readonly string _expressionName;

    private readonly IJsonAbstractExpressionFactory _factory;

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public AbstractJsonBinaryExpressionFactory(string expressionName, IJsonAbstractExpressionFactory factory)
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

        IExpression<Task<TLeft>> leftExpression = _factory.Create<IExpression<Task<TLeft>>>(leftInstruction);
        IExpression<Task<TRight>> rightExpression = _factory.Create<IExpression<Task<TRight>>>(rightInstruction);

        return CreateExpressionInstance(leftExpression, rightExpression);
    }

    protected abstract TOut CreateExpressionInstance(IExpression<Task<TLeft>> leftExpression, IExpression<Task<TRight>> rightExpression);
}
