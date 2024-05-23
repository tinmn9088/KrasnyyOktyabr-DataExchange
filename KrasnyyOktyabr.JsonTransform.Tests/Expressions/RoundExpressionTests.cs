using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class RoundExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RoundExpression_WhenValueExpressionNull_ShouldThrowArgumentNullException()
    {
        new RoundExpression(null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnRounded()
    {
        Number valueExpressionResult = new(2.4M);

        // Setting up value expression
        Mock<IExpression<Task<Number>>> valueExpressionMock = new();
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(valueExpressionResult));

        RoundExpression roundExpression = new(valueExpressionMock.Object);

        Number expected = new(2);

        Number actual = await roundExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnRoundAwayFromZero()
    {
        Number valueExpressionResult = new(2.5M);

        // Setting up value expression
        Mock<IExpression<Task<Number>>> valueExpressionMock = new();
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(valueExpressionResult));

        RoundExpression roundExpression = new(valueExpressionMock.Object);

        Number expected = new(3);

        Number actual = await roundExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenDigitsSpecified_ShouldReturnRounded()
    {
        Number valueExpressionResult = new(2.666M);

        // Setting up value expression
        Mock<IExpression<Task<Number>>> valueExpressionMock = new();
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(valueExpressionResult));

        // Setting up digits expression
        Mock<IExpression<Task<long>>> digitsExpressionMock = new();
        digitsExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        RoundExpression roundExpression = new(valueExpressionMock.Object, digitsExpressionMock.Object);

        Number expected = new(2.67M);

        Number actual = await roundExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
