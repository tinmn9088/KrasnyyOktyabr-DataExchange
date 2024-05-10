using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class StringFormatExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringFormatExpression_WhenFormatExpressionNull_ShouldThrowArgumentNullException()
    {
        new StringFormatExpression(null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringFormatExpression_WhenArgExpressionsNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<string>>> formatExpressionMock = new();

        new StringFormatExpression(formatExpressionMock.Object, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void StringFormatExpression_WhenOneOfArgExpressionsNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<string>>> formatExpressionMock = new();

        new StringFormatExpression(formatExpressionMock.Object, [null!]);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldFormatString()
    {
        // Setting up format expression
        Mock<IExpression<Task<string>>> formatExpressionMock = new();
        string format = "Hello {0}!";
        formatExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(format));

        // Setting up arg0 expression
        Mock<IExpression<Task<object?>>> arg0ExpressionMock = new();
        string arg0 = "World";
        arg0ExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)arg0));

        // Setting up string format expression
        StringFormatExpression stringFormatExpression = new(formatExpressionMock.Object, [arg0ExpressionMock.Object]);

        Context context = CreateEmptyExpressionContext();

        string expected = string.Format(format, [arg0]);

        string actual = await stringFormatExpression.InterpretAsync(context);

        formatExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        arg0ExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenFormatExpressionResultNull_ShouldThrowInterpretException()
    {
        // Setting up format expression
        Mock<IExpression<Task<string>>> nullFormatExpressionMock = new();
        string format = null!;
        nullFormatExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(format));

        // Setting up string format expression
        StringFormatExpression stringFormatExpression = new(nullFormatExpressionMock.Object, []);

        await stringFormatExpression.InterpretAsync(CreateEmptyExpressionContext());
    }
}
