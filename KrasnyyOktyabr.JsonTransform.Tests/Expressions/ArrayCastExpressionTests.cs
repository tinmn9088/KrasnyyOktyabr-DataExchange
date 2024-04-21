using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ArrayCastExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ArrayCastExpression_WhenInnerExpressionNull_ShouldThrowArgumentNullException()
    {
        new ArrayCastExpression(null!);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenInnerExpressionResultObjectArray_ShouldReturnArray()
    {
        object?[] expected = ["TestValue"];

        // Setting up inner expression mock that returns objects array
        Mock<IExpression<Task<object?[]>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        ArrayCastExpression expression = new(innerExpressionMock.Object);

        object?[] actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenInnerExpressionResultStringArray_ShouldReturnArray()
    {
        string[] expected = ["TestValue"];

        // Setting up inner expression mock that returns objects array
        Mock<IExpression<Task<string[]>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        ArrayCastExpression expression = new(innerExpressionMock.Object);

        object?[] actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreSame(expected, actual);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenInnerExpressionResultIEnumerable_ShouldReturnArray()
    {
        JToken arrayItem = "TestValue";
        JArray expected = ["TestValue"];

        // Setting up inner expression mock that returns objects array
        Mock<IExpression<Task<JArray>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        ArrayCastExpression expression = new(innerExpressionMock.Object);

        object?[] actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(arrayItem, actual[0]);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenInnerExpressionResultObject_ShouldReturnWrapInArray()
    {
        string expectedArrayItem = "TestValue";

        // Setting up inner expression mock that returns objects array
        Mock<IExpression<Task<string>>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedArrayItem);

        ArrayCastExpression expression = new(innerExpressionMock.Object);

        object?[] actual = await expression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreSame(expectedArrayItem, actual[0]);
    }
}
