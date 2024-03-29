using Newtonsoft.Json.Linq;
using NJsonSchema;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonExpressionsBlockFactory(IJsonAbstractExpressionFactory factory) : IJsonExpressionFactory<ExpressionsBlock>
{
    private static readonly Lazy<JsonSchema> s_jsonSchema = new(() =>
        JsonSchema.FromJsonAsync(@"{
              'type': 'array',
              'items': {
                'type': {
                  'anyOf': [
                    'object',
                    'array'
                  ]
                }
              }
            }").Result);

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"><paramref name="input"/> is not <see cref="JArray"/>.</exception>
    public ExpressionsBlock Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input is JArray instructions)
        {
            List<IExpression<Task>> expressions = instructions
                .Select(factory.Create<IExpression<Task>>)
                .ToList();

            return new ExpressionsBlock(expressions);
        }
        else
        {
            throw new ArgumentException($"'{nameof(input)}' must be array");
        }
    }

    public bool Match(JToken value)
    {
        return s_jsonSchema.Value.Validate(value).Count == 0;
    }
}
