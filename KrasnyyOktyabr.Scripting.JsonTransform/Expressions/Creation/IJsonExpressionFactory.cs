using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Creation;

public interface IJsonExpressionFactory<out TOut> : IExpressionFactory<JToken, TOut> where TOut : IExpression<Task>
{
}
