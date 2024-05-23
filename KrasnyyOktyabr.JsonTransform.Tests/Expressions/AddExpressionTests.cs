using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class AddExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddExpression_WhenKeyExpressionNull_ShouldThrowArgumentNullException()
    {
        new AddExpression(null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AddExpression_WhenValueExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<string>>> keyExpressionMock = new();

        new AddExpression(keyExpressionMock.Object, null!);
    }

    [TestMethod]
    public void AddExpression_WhenIndexExpressionNull_ShouldCreateAddExpression()
    {
        Mock<IExpression<Task<string>>> pathExpressionMock = new();
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();

        new AddExpression(pathExpressionMock.Object, valueExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldAdd()
    {
        // Setting up key expression
        Mock<IExpression<Task<string>>> keyExpressionMock = new();
        string keyString = "TestKey";
        keyExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(keyString));

        // Setting up value expression
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        string expectedValue = "TestValue";
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expectedValue));

        // Setting up select expression
        AddExpression addExpression = new(keyExpressionMock.Object, valueExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        await addExpression.InterpretAsync(context);

        List<JObject> output = context.OutputGet();

        Assert.AreEqual(1, output.Count);
        Assert.IsTrue(output[0].ContainsKey(keyString));
        Assert.AreEqual(expectedValue, output[0][keyString]);
        keyExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        valueExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
    }

    [TestMethod]
    public async Task InterpretAsync_WhenIndexSpecified_ShouldAdd()
    {
        // Setting up key expression
        Mock<IExpression<Task<string>>> keyExpressionMock = new();
        string keyString = "TestKey";
        keyExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(keyString));

        // Setting up value expression
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        string expectedValue = "TestValue";
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expectedValue));

        // Setting up index expression
        Mock<IExpression<Task<long>>> indexExpressionMock = new();
        int index = 2;
        indexExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Convert.ToInt64(index));

        // Setting up select expression
        AddExpression addExpression = new(keyExpressionMock.Object, valueExpressionMock.Object, indexExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        await addExpression.InterpretAsync(context);

        List<JObject> output = context.OutputGet();

        Assert.AreEqual(index + 1, output.Count);
        Assert.IsTrue(output[index].ContainsKey(keyString));
        Assert.AreEqual(expectedValue, output[index][keyString]);
        keyExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        valueExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenKeyNull_ShouldThrowInterpretException()
    {
        // Setting up null key expression
        Mock<IExpression<Task<string>>> keyExpressionMock = new();
        keyExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((string)null!));

        // Setting up value expression
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        object? expectedValue = "TestValue";
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expectedValue));

        // Setting up select expression
        AddExpression addExpression = new(keyExpressionMock.Object, valueExpressionMock.Object);

        await addExpression.InterpretAsync(CreateEmptyExpressionContext());
    }
}
