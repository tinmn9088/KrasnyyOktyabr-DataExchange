using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public sealed class JsonConstStringExpressionFactory : AbstractJsonExpressionFactory<ConstStringExpression>
{
    public JsonConstStringExpressionFactory()
        : base(@"{
              'type': 'string'
            }")
    {
    }

    /// <exception cref="NullReferenceException"></exception>
    public override ConstStringExpression Create(JToken input) => new(input.Value<string>() ?? throw new NullReferenceException());
}

