using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonConstIntExpressionFactory : AbstractJsonExpressionFactory<ConstIntExpression>
{
    public JsonConstIntExpressionFactory()
        : base(@"{
              'type': 'integer'
            }")
    {
    }

    public override ConstIntExpression Create(JToken input) => new(input.Value<int>());
}

