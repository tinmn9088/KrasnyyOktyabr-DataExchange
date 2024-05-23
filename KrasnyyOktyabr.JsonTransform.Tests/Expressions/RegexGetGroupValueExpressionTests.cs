using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class RegexGetGroupValueExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegexGetGroupValueExpression_WhenRegexExpressionNull_ShouldThrowArgumentNullException()
    {
        new RegexGetGroupValueExpression(null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RegexGetGroupValueExpression_WhenAInputExpressionsNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<string>>> inputExpressionMock = new();

        new RegexGetGroupValueExpression(inputExpressionMock.Object, null!);
    }

    [TestMethod]
    public void RegexGetGroupValueExpression_WhenAGroupNumberExpressionsNull_ShouldCreateRegexGetGroupValueExpression()
    {
        Mock<IExpression<Task<string>>> inputExpressionMock = new();
        Mock<IExpression<Task<string>>> regexExpressionMock = new();

        new RegexGetGroupValueExpression(inputExpressionMock.Object, regexExpressionMock.Object, null);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldGetGroupValue()
    {
        // Setting up regex expression
        Mock<IExpression<Task<string>>> regexExpressionMock = new();
        string regexString = "Hello, (.*)\\!";
        regexExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(regexString));

        // Setting up input expression
        Mock<IExpression<Task<string>>> inputExpressionMock = new();
        string input = "Hello, World!";
        inputExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(input));

        // Setting up regex get group value expression
        RegexGetGroupValueExpression regexGetGroupValueExpression = new(regexExpressionMock.Object, inputExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        string expected = "World";

        string actual = await regexGetGroupValueExpression.InterpretAsync(context);

        regexExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        inputExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenGroupNumberExpressionPassed_ShouldGetGroupValueWithNumber()
    {
        // Setting up regex expression
        Mock<IExpression<Task<string>>> regexExpressionMock = new();
        string regexString = "Hello, (.*) and (.*)\\!";
        regexExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(regexString));

        // Setting up input expression
        Mock<IExpression<Task<string>>> inputExpressionMock = new();
        string input = "Hello, World and everybody!";
        inputExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(input));

        // Setting up group number expression
        Mock<IExpression<Task<long>>> groupNumberExpressionMock = new();
        int groupNumber = 2;
        groupNumberExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(groupNumber);

        // Setting up regex get group value expression
        RegexGetGroupValueExpression regexGetGroupValueExpression = new(regexExpressionMock.Object, inputExpressionMock.Object, groupNumberExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        string expected = "everybody";

        string actual = await regexGetGroupValueExpression.InterpretAsync(context);

        regexExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        inputExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenRegexExpressionResultNull_ShouldThrowInterpretException()
    {
        // Setting up null regex expression
        Mock<IExpression<Task<string>>> nullRegexExpressionMock = new();
        nullRegexExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((string)null!));

        // Setting up input expression
        Mock<IExpression<Task<string>>> inputExpressionMock = new();
        string input = "Hello, World!";
        inputExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(input));

        // Setting up regex get group value expression
        RegexGetGroupValueExpression regexGetGroupValueExpression = new(nullRegexExpressionMock.Object, inputExpressionMock.Object);

        await regexGetGroupValueExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenInputExpressionResultNull_ShouldThrowInterpretException()
    {
        // Setting up regex expression
        Mock<IExpression<Task<string>>> regexExpressionMock = new();
        string regexString = "Hello, (.*)\\!";
        regexExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(regexString));

        // Setting up null input expression
        Mock<IExpression<Task<string>>> nullInputExpressionMock = new();
        nullInputExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((string)null!));

        // Setting up regex get group value expression
        RegexGetGroupValueExpression regexGetGroupValueExpression = new(regexExpressionMock.Object, nullInputExpressionMock.Object);

        await regexGetGroupValueExpression.InterpretAsync(CreateEmptyExpressionContext());
    }
}
