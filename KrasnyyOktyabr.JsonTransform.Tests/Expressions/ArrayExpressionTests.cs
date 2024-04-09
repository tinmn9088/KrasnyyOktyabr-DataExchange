using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ArrayExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ArrayExpression_WhenExpressionsNull_ShouldThrowArgumentNullException()
    {
        new ArrayExpression(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ArrayExpression_WhenExpressionsIncludeNull_ShouldThrowArgumentNullException()
    {
        new ArrayExpression([null!]);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldRunExpressions()
    {
        string testValue = "TestValue";

        // Setting up expression mock
        Mock<IExpression<Task<object?>>> expressionMock = new();
        expressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)testValue));

        // Setting up ArrayExpression with its content
        List<IExpression<Task<object?>>> expressions = [expressionMock.Object];
        ArrayExpression arrayExpression = new(expressions);

        object?[] actual = await arrayExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(1, actual.Length);
        Assert.AreEqual(testValue, actual[0]);
        expressionMock.Verify(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()), Times.Once());
        expressionMock.VerifyNoOtherCalls();
    }
}
