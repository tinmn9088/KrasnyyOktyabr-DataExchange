using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class SumExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void SumExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();

        new SumExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void SumExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();

        new SumExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnResult()
    {
        Number leftExpressionResult = new(2);
        Number rightExpressionResult = new(3);

        // Setting up left expression
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();
        leftExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(leftExpressionResult));

        // Setting up right expression
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();
        rightExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(rightExpressionResult));

        SumExpression sumExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        Number expected = leftExpressionResult + rightExpressionResult;

        Number actual = await sumExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
