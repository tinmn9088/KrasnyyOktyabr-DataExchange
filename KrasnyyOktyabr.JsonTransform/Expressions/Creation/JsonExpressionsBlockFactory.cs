using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <remarks>
/// Overlaps JSON Schema of <see cref="JsonArrayExpressionFactory"/>!
/// </remarks>
public sealed class JsonExpressionsBlockFactory : AbstractJsonExpressionFactory<ExpressionsBlock>
{
    private readonly IJsonAbstractExpressionFactory _factory;

    public JsonExpressionsBlockFactory(IJsonAbstractExpressionFactory factory)
        : base(@"{
              'type': 'array'
            }")
    {
        ArgumentNullException.ThrowIfNull(factory);

        _factory = factory;
    }

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"><paramref name="input"/> is not <see cref="JArray"/>.</exception>
    public override ExpressionsBlock Create(JToken input)
    {
        ArgumentNullException.ThrowIfNull(input);

        if (input is JArray instructions)
        {
            List<IExpression<Task>> expressions = instructions
                .Select(_factory.Create<IExpression<Task>>)
                .ToList();

            return new(expressions);
        }
        else
        {
            throw new ArgumentException($"'{nameof(input)}' must be array");
        }
    }
}
