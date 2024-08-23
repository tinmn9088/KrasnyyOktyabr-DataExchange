using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <remarks>
/// Overlaps JSON Schema of <see cref="JsonConstLongExpressionFactory"/>!
/// </remarks>
public sealed class JsonConstDecimalExpressionFactory : AbstractJsonExpressionFactory<ConstDecimalExpression>
{
    public JsonConstDecimalExpressionFactory()
        : base(@"{
              'type': 'number'
            }")
    {
    }

    public override ConstDecimalExpression Create(JToken input) => new(input.Value<decimal>());
}

