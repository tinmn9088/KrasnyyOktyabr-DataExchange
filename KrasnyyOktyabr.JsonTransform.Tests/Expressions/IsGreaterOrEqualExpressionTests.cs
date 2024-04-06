using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class IsGreaterOrEqualExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IsGreaterOrEqualExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();

        new IsGreaterOrEqualExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IsGreaterOrEqualExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();

        new IsGreaterOrEqualExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenGreater_ShouldReturnTrue()
    {
        Number leftExpressionResult = new(3);
        Number rightExpressionResult = new(2);

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

        IsGreaterOrEqualExpression isGreaterOrEqualExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        bool result = await isGreaterOrEqualExpression.InterpretAsync(GetEmptyExpressionContext());

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenEqual_ShouldReturnTrue()
    {
        Number leftExpressionResult = new(3);
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

        IsGreaterOrEqualExpression isGreaterOrEqualExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        bool result = await isGreaterOrEqualExpression.InterpretAsync(GetEmptyExpressionContext());

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenNotGreater_ShouldReturnFalse()
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

        IsGreaterOrEqualExpression isGreaterOrEqualExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        bool result = await isGreaterOrEqualExpression.InterpretAsync(GetEmptyExpressionContext());

        Assert.IsFalse(result);
    }
}
