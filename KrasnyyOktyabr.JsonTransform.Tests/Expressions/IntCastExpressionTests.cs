using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class IntCastExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IntCastExpression_WhenInnerNull_ShouldThrowArgumentNullException()
    {
        new IntCastExpression(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public async Task InterpretAsync_WhenInnerExpressionResultNull_ShouldThrowIntCastExpressionException()
    {
        // Setting up inner expression mock that returns null
        Mock<IExpression<Task<object?>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<object?>(null));

        IntCastExpression expression = new(innerExpressionMock.Object);

        await expression.InterpretAsync(GetEmptyExpressionContext());
    }

    [TestMethod]
    [ExpectedException(typeof(IntCastExpression.IntCastExpressionException))]
    public async Task InterpretAsync_WhenInnerExpressionResultNotInt_ShouldThrowNullReferenceException()
    {
        // Setting up inner expression mock that returns char
        Mock<IExpression<Task<char>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult('c'));

        IntCastExpression expression = new(innerExpressionMock.Object);

        await expression.InterpretAsync(GetEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_WhenInnerExpressionResultIntString_ShouldParseStringAndReturnInt()
    {
        int expected = 66;

        // Setting up inner expression mock that returns integer string
        Mock<IExpression<Task<string>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expected.ToString()));

        IntCastExpression expression = new(innerExpressionMock.Object);

        int actual = await expression.InterpretAsync(GetEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnInt()
    {
        int expected = 99;

        // Setting up inner expression mock that returns integer
        Mock<IExpression<Task<int>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expected));

        IntCastExpression expression = new(innerExpressionMock.Object);

        int actual = await expression.InterpretAsync(GetEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
