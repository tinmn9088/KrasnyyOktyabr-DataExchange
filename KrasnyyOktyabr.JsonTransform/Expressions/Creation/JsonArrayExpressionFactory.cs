using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonArrayExpressionFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ArrayExpression>(@"{
            'type': 'array'
        }")
{
    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"><paramref name="input"/> is not <see cref="JArray"/>.</exception>
    public override ArrayExpression Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (input is JArray instructions)
        {
            List<IExpression<Task<object?>>> expressions = instructions
                .Select(_factory.Create<IExpression<Task<object?>>>)
                .ToList();

            return new(expressions);
        }

        throw new ArgumentException($"'{nameof(input)}' must be array");
    }
}
