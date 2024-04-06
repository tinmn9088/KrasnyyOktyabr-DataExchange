using System.Globalization;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class DoubleCastExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DoubleCastExpression_WhenInnerExpressionNull_ShouldThrowArgumentNullException()
    {
        new DoubleCastExpression(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public async Task InterpretAsync_WhenInnerExpressionResultNull_ShouldThrowNullReferenceException()
    {
        // Setting up inner expression mock that returns null
        Mock<IExpression<Task<object?>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<object?>(null));

        DoubleCastExpression expression = new(innerExpressionMock.Object);

        await expression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    [ExpectedException(typeof(DoubleCastExpression.DoubleCastExpressionException))]
    public async Task InterpretAsync_WhenInnerExpressionResultNotDouble_ShouldThrowDoubleCastExpressionException()
    {
        // Setting up inner expression mock that returns char
        Mock<IExpression<Task<char>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult('c'));

        DoubleCastExpression expression = new(innerExpressionMock.Object);

        await expression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_WhenInnerExpressionResultDoubleString_ShouldParseStringAndReturnDouble()
    {
        double expected = 66.6;

        // Setting up inner expression mock that returns double string
        Mock<IExpression<Task<string>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expected.ToString(CultureInfo.InvariantCulture)));

        DoubleCastExpression expression = new(innerExpressionMock.Object);

        double actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnDouble()
    {
        double expected = 99.9;

        // Setting up inner expression mock that returns integer
        Mock<IExpression<Task<double>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(expected));

        DoubleCastExpression expression = new(innerExpressionMock.Object);

        double actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
