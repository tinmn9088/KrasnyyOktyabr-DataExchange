using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ConstExpressionTests
{
    [TestMethod]
    public void InterpretAsync_ShouldReturnSameTaskInstance()
    {
        Context context = CreateEmptyExpressionContext();

        ConstExpression expression = new(null!);

        Task<object?> result1 = expression.InterpretAsync(context);
        Task<object?> result2 = expression.InterpretAsync(context);

        Assert.AreSame(result1, result2);
    }
}
