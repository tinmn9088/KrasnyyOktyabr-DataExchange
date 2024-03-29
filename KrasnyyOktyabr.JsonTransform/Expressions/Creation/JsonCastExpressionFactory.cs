using Newtonsoft.Json.Linq;
using NJsonSchema;
using static KrasnyyOktyabr.JsonTransform.Expressions.Creation.JsonExpressionFactoriesHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonCastExpressionFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<IExpression<Task>>
{
    public enum ReturnType
    {
        Int,
        Float,
        Bool,
        String,
    }

    public static string JsonSchemaPropertyCast => "$cast";

    public static string JsonSchemaPropertyType => "type";

    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() =>
        JsonSchema.FromJsonAsync(@"{
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
            }").Result);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public IExpression<Task> Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        JObject instruction = (JObject)input[JsonSchemaPropertyCast]!;
        JToken valueInstruction = instruction[JsonSchemaPropertyValue]!;

        string returnTypeString = instruction[JsonSchemaPropertyType]!.Value<string>()!;

        // No InvalidOperationException because of JSON Schema
        ReturnType returnType = (ReturnType)Enum.Parse(typeof(ReturnType), returnTypeString, ignoreCase: true);

        IExpression<Task> innerExpression = factory.Create<IExpression<Task>>(valueInstruction);

        return returnType switch
        {
            ReturnType.Int => new IntCastExpression(innerExpression),
            ReturnType.Float => new DoubleCastExpression(innerExpression),
            ReturnType.Bool => new BoolCastExpression(innerExpression),
            ReturnType.String => new StringCastExpression(innerExpression),
            _ => throw new ArgumentException($"Unexpected '{JsonSchemaPropertyType}' value: '{returnTypeString}'"),
        };
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }

    private static string GetPropertyTypeValuesString()
    {
        return Enum.GetValues<ReturnType>()
            .Select(t => $"'{t.ToString().ToLower()}'")
            .Aggregate((types, next) => $"{types}, {next}");
    }
}
