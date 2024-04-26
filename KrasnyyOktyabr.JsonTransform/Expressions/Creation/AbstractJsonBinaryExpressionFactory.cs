using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <exception cref="ArgumentException"></exception>
/// <exception cref="ArgumentNullException"></exception>
public abstract class AbstractJsonBinaryExpressionFactory<TLeft, TRight, TOut>(string expressionName, IJsonAbstractExpressionFactory factory)
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
        JToken leftInstruction = instruction[JsonSchemaPropertyLeft]!;
        JToken rightInstruction = instruction[JsonSchemaPropertyRight]!;

        IExpression<Task<TLeft>> leftExpression = _factory.Create<IExpression<Task<TLeft>>>(leftInstruction);
        IExpression<Task<TRight>> rightExpression = _factory.Create<IExpression<Task<TRight>>>(rightInstruction);

        return CreateExpressionInstance(leftExpression, rightExpression);
    }

    protected abstract TOut CreateExpressionInstance(IExpression<Task<TLeft>> leftExpression, IExpression<Task<TRight>> rightExpression);
}
