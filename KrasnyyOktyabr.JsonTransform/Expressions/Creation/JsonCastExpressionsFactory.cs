using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonCastExpressionsFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<IExpression<Task>>(@"{
            'type': 'object',
            'additionalProperties': false,
            'properties': {
            '" + JsonSchemaPropertyComment + @"': {
                'type': 'string'
            },
            '" + JsonSchemaPropertyCast + @"': {
                'type': 'object',
                'additionalProperties': false,
                'properties': {
                '" + JsonSchemaPropertyValue + @"': {},
                '" + JsonSchemaPropertyType + @"': {
                    'type': 'string',
                    'enum': [" + GetPropertyTypeValuesString() + @"]
                }
                },
                'required': [
                '" + JsonSchemaPropertyValue + @"',
                '" + JsonSchemaPropertyType + @"',
                ]
            }
            },
            'required': [
            '" + JsonSchemaPropertyCast + @"'
            ]
        }")
{
    public enum ReturnType
    {
        Int,
        Float,
        Bool,
        String,
        Array,
    }

    public static string JsonSchemaPropertyCast => "$cast";

    public static string JsonSchemaPropertyType => "type";

    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public override IExpression<Task> Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        JObject instruction = (JObject)input[JsonSchemaPropertyCast]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;

        string returnTypeString = instruction[JsonSchemaPropertyType]!.Value<string>()!;

        // No InvalidOperationException because of JSON Schema
        ReturnType returnType = (ReturnType)Enum.Parse(typeof(ReturnType), returnTypeString, ignoreCase: true);

        IExpression<Task> innerExpression = _factory.Create<IExpression<Task>>(valueInstruction);

        return returnType switch
        {
            ReturnType.Int => new IntCastExpression(innerExpression),
            ReturnType.Float => new DoubleCastExpression(innerExpression),
            ReturnType.Bool => new BoolCastExpression(innerExpression),
            ReturnType.String => new StringCastExpression(innerExpression),
            ReturnType.Array => new ArrayCastExpression(innerExpression),
            _ => throw new ArgumentException($"Unexpected '{JsonSchemaPropertyType}' value: '{returnTypeString}'"),
        };
    }

    private static string GetPropertyTypeValuesString()
    {
        List<string> returnTypeNames = [];

        foreach (object name in Enum.GetValues(typeof(ReturnType)))
        {
            returnTypeNames.Add($"'{name.ToString().ToLower()}'");
        }

        return string.Join(", ", [.. returnTypeNames]);
    }
}
