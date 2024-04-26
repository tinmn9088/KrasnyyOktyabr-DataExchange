using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public abstract class AbstractJsonExpressionFactory<TOut> : IJsonExpressionFactory<TOut> where TOut : IExpression<Task>
{
    private readonly Lazy<JsonSchema> _jsonSchema;

    public AbstractJsonExpressionFactory(string jsonSchemaString)
    {
        if (jsonSchemaString == null || string.IsNullOrWhiteSpace(jsonSchemaString))
        {
            throw new ArgumentNullException(nameof(jsonSchemaString));
        }

        _jsonSchema = new Lazy<JsonSchema>(() => JsonSchema.FromJsonAsync(jsonSchemaString).Result);
    }

    public abstract TOut Create(JToken input);

    public bool Match(JToken value)
    {
        return _jsonSchema.Value.Validate(value).Count == 0;
    }
}
