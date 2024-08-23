using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonRegexGetGroupValueExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<RegexGetGroupValueExpression>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertyRegexGetGroup + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyRegex + @"': {},
                '" + JsonSchemaPropertyInput + @"': {},
                '" + JsonSchemaPropertyGroupNumber + @"': {}
                },
                'required': [
                '" + JsonSchemaPropertyRegex + @"',
                '" + JsonSchemaPropertyInput + @"'
                ]
            }
            },
            'required': [
            '" + JsonSchemaPropertyRegexGetGroup + @"'
            ]
        }")
{
    public static string JsonSchemaPropertyRegexGetGroup => "$regexgetgroup";

    public static string JsonSchemaPropertyRegex => "regex";

    public static string JsonSchemaPropertyInput => "input";

    public static string JsonSchemaPropertyGroupNumber => "groupNumber";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    public override RegexGetGroupValueExpression Create(JToken input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyRegexGetGroup]!;
        JToken regexInstruction = instruction[JsonSchemaPropertyRegex]!;
        JToken inputInstruction = instruction[JsonSchemaPropertyInput]!;
        JToken? groupNumberInstruction = instruction[JsonSchemaPropertyGroupNumber];

        IExpression<Task<string>> regexExpression = _factory.Create<IExpression<Task<string>>>(regexInstruction);
        IExpression<Task<string>> inputExpression = _factory.Create<IExpression<Task<string>>>(inputInstruction);

        if (groupNumberInstruction is not null)
        {
            IExpression<Task<long>> groupNumberExpression = _factory.Create<IExpression<Task<long>>>(groupNumberInstruction);

            return new(regexExpression, inputExpression, groupNumberExpression);
        }
        else
        {
            return new(regexExpression, inputExpression);
        }
    }
}
