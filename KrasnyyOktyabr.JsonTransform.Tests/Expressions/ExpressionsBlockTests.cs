using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ExpressionsBlockTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ExpressionsBlock_WhenExpressionsNull_ShouldThrowArgumentNullException()
    {
        new ExpressionsBlock(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ExpressionsBlock_WhenExpressionsIncludeNull_ShouldThrowArgumentNullException()
    {
        new ExpressionsBlock([null!]);
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

        // Setting up ExpressionsBlock with its content
        List<IExpression<Task<object?>>> expressions = [expressionMock.Object];
        ExpressionsBlock expressionsBlock = new(expressions);

        object?[] actual = await expressionsBlock.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(1, actual.Length);
        Assert.AreEqual(testValue, actual[0]);
        expressionMock.Verify(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()), Times.Once());
        expressionMock.VerifyNoOtherCalls();
    }
}
