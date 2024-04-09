using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonArrayExpressionFactory : AbstractJsonExpressionFactory<ArrayExpression>
{
    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonArrayExpressionFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'array'
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"><paramref name="input"/> is not <see cref="JArray"/>.</exception>
    public override ArrayExpression Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input is JArray instructions)
        {
            List<IExpression<Task<object?>>> expressions = instructions
                .Select(_factory.Create<IExpression<Task<object?>>>)
                .ToList();

            return new ArrayExpression(expressions);
        }
        else
        {
            throw new ArgumentException($"'{nameof(input)}' must be array");
        }
    }
}
