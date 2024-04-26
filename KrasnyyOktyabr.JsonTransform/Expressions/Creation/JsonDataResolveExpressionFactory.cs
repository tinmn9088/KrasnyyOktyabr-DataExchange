using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonDataResolveExpressionFactory(IJsonAbstractExpressionFactory factory, IDataResolveService dataResolveService)
    : AbstractJsonExpressionFactory<DataResolveExpression>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertyResolve + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyResolver + @"': {},
                '" + JsonSchemaPropertyParams + @"': {}
                },
                'required': [
                '" + JsonSchemaPropertyResolver + @"',
                '" + JsonSchemaPropertyParams + @"'
                ]
            }
            },
            'required': [
            '" + JsonSchemaPropertyResolve + @"'
            ]
        }")
{
    public static string JsonSchemaPropertyResolve => "$resolve";

    public static string JsonSchemaPropertyResolver => "resolver";

    public static string JsonSchemaPropertyParams => "params";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    private readonly IDataResolveService _dataResolveService = dataResolveService ?? throw new ArgumentNullException(nameof(dataResolveService));

    /// <exception cref="ArgumentNullException"></exception>
    public override DataResolveExpression Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyResolve]!;
        JToken resolverInstruction = instruction[JsonSchemaPropertyResolver]!;
        JToken paramsInstructions = instruction[JsonSchemaPropertyParams]!;

        IExpression<Task<string>> resolverExpression = _factory.Create<IExpression<Task<string>>>(resolverInstruction);
        IExpression<Task<Dictionary<string, object?>>> paramsExpression = _factory.Create<IExpression<Task<Dictionary<string, object?>>>>(paramsInstructions);

        return new(resolverExpression, paramsExpression, _dataResolveService);
    }
}
