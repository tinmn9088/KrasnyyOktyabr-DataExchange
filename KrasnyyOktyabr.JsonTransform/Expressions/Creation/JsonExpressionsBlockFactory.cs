using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <remarks>
/// Overlaps JSON Schema of <see cref="JsonArrayExpressionFactory"/>!
/// </remarks>
public sealed class JsonExpressionsBlockFactory(IJsonAbstractExpressionFactory factory)
    : AbstractJsonExpressionFactory<ExpressionsBlock>(@"{
              'type': 'array'
            }")
{
    private readonly IJsonAbstractExpressionFactory _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"><paramref name="input"/> is not <see cref="JArray"/>.</exception>
    public override ExpressionsBlock Create(JToken input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

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
