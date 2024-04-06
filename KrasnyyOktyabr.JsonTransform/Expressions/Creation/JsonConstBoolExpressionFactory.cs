using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonConstBoolExpressionFactory : AbstractJsonExpressionFactory<ConstBoolExpression>
{
    public JsonConstBoolExpressionFactory()
        : base(@"{
              'type': 'boolean'
            }")
    {
    }

    public override ConstBoolExpression Create(JToken input) => new(input.ToObject<bool>());
}

