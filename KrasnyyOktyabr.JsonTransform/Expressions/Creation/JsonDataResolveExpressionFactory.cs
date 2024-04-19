using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonDataResolveExpressionFactory : AbstractJsonExpressionFactory<DataResolveExpression>
{
    public static string JsonSchemaPropertyResolve => "$resolve";

    public static string JsonSchemaPropertyResolver => "resolver";

    public static string JsonSchemaPropertyParams => "params";

    private readonly IJsonAbstractExpressionFactory _factory;

    private readonly IDataResolveService _dataResolveService;

    public JsonDataResolveExpressionFactory(IJsonAbstractExpressionFactory factory, IDataResolveService dataResolveService)
        : base(@"{
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
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(dataResolveService);

        _factory = factory;
        _dataResolveService = dataResolveService;
    }

    /// <exception cref="ArgumentNullException"></exception>
    public override DataResolveExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyResolve]!;
        JToken resolverInstruction = instruction[JsonSchemaPropertyResolver]!;
        JToken paramsInstructions = instruction[JsonSchemaPropertyParams]!;

        IExpression<Task<string>> resolverExpression = _factory.Create<IExpression<Task<string>>>(resolverInstruction);
        IExpression<Task<Dictionary<string, object?>>> paramsExpression = _factory.Create<IExpression<Task<Dictionary<string, object?>>>>(paramsInstructions);

        return new DataResolveExpression(resolverExpression, paramsExpression, _dataResolveService);
    }
}
