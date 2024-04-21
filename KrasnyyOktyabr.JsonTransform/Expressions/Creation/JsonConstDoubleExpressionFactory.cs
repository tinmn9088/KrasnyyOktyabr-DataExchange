using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

/// <remarks>
/// Overlaps JSON Schema of <see cref="JsonConstIntExpressionFactory"/>!
/// </remarks>
public sealed class JsonConstDoubleExpressionFactory : AbstractJsonExpressionFactory<ConstDoubleExpression>
{
    public JsonConstDoubleExpressionFactory()
        : base(@"{
              'type': 'number'
            }")
    {
    }

    public override ConstDoubleExpression Create(JToken input) => new(input.Value<double>());
}

