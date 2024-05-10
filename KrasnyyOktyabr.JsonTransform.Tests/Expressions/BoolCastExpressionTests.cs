using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class BoolCastExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BoolCastExpression_WhenInnerExpressionNull_ShouldThrowArgumentNullException()
    {
        new BoolCastExpression(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenInnerExpressionResultNull_ShouldThrowBoolInterpretException()
    {
        // Setting up inner expression mock that returns null
        Mock<IExpression<Task<object?>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<object?>(null));

        BoolCastExpression expression = new(innerExpressionMock.Object);

        await expression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    [ExpectedException(typeof(BoolCastExpression.BoolCastExpressionException))]
    public async Task InterpretAsync_WhenInnerExpressionResultNotBool_ShouldThrowBoolCastExpressionException()
    {
        // Setting up inner expression mock that returns char
        Mock<IExpression<Task<char>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult('c'));

        BoolCastExpression expression = new(innerExpressionMock.Object);

        await expression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_WhenInnerExpressionResultBoolString_ShouldParseStringAndReturnBool()
    {
        bool expected = true;

        // Setting up inner expression mock that returns bool string
        Mock<IExpression<Task<string>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expected.ToString()));

        BoolCastExpression expression = new(innerExpressionMock.Object);

        bool actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnBool()
    {
        bool expected = false;

        // Setting up inner expression mock that returns integer
        Mock<IExpression<Task<bool>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expected));

        BoolCastExpression expression = new(innerExpressionMock.Object);

        bool actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
