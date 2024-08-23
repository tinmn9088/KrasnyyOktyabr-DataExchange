using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonConstLongExpressionFactory : AbstractJsonExpressionFactory<ConstLongExpression>
{
    public JsonConstLongExpressionFactory()
        : base(@"{
              'type': 'integer'
            }")
    {
    }

    public override ConstLongExpression Create(JToken input) => new(input.Value<long>());
}

