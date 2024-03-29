using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ConstExpressionTests
{
    [TestMethod]
    public void InterpretAsync_ShouldReturnSameTaskInstance()
    {
        JObject contextInput = new();
        Context context = new(contextInput);

        ConstExpression expression = new(null!);

        Task<object?> result1 = expression.InterpretAsync(context);
        Task<object?> result2 = expression.InterpretAsync(context);

        Assert.AreSame(result1, result2);
    }
}
