using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonRegexGetGroupValueExpressionFactory : AbstractJsonExpressionFactory<RegexGetGroupValueExpression>
{
    public static string JsonSchemaPropertyRegexGetGroup => "$regexgetgroup";

    public static string JsonSchemaPropertyRegex => "regex";

    public static string JsonSchemaPropertyInput => "input";

    public static string JsonSchemaPropertyGroupNumber => "groupNumber";

    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonRegexGetGroupValueExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
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
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override RegexGetGroupValueExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyRegexGetGroup]!;
        JToken regexInstruction = instruction[JsonSchemaPropertyRegex]!;
        JToken inputInstruction = instruction[JsonSchemaPropertyInput]!;
        JToken? groupNumberInstruction = instruction[JsonSchemaPropertyGroupNumber];

        IExpression<Task<string>> regexExpression = _factory.Create<IExpression<Task<string>>>(regexInstruction);
        IExpression<Task<string>> inputExpression = _factory.Create<IExpression<Task<string>>>(inputInstruction);

        if (groupNumberInstruction != null)
        {
            IExpression<Task<int>> groupNumberExpression = _factory.Create<IExpression<Task<int>>>(groupNumberInstruction);

            return new(regexExpression, inputExpression, groupNumberExpression);
        }
        else
        {
            return new(regexExpression, inputExpression);
        }
    }
}
