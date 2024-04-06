using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class SubstractExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void SubstractExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();

        new SubstractExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void SubstractExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();

        new SubstractExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnSum()
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

        SubstractExpression substractExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        Number expected = leftExpressionResult - rightExpressionResult;

        Number actual = await substractExpression.InterpretAsync(GetEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
