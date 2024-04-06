using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class IsGreaterExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IsGreaterExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> rightExpressionMock = new();

        new IsGreaterExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IsGreaterExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<Number>>> leftExpressionMock = new();

        new IsGreaterExpression(leftExpressionMock.Object, null!);
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

        IsGreaterExpression isGreaterExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        bool result = await isGreaterExpression.InterpretAsync(GetEmptyExpressionContext());

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

        IsGreaterExpression isGreaterExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        bool result = await isGreaterExpression.InterpretAsync(GetEmptyExpressionContext());

        Assert.IsFalse(result);
    }
}
