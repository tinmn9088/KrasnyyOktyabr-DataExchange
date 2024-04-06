using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class MapExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MapExpression_WhenExpressionsNull_ShouldThrowArgumentNullException()
    {
        new MapExpression(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MapExpression_WhenExpressionsIncludeNull_ShouldThrowArgumentNullException()
    {
        new MapExpression(new Dictionary<string, IExpression<Task<object?>>>() {
            { "Key", null! },
        });
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldRunExpressions()
    {
        string expressionKey = "Key";
        string expressionResult = "TestResult";

        // Setting up expression mock
        Mock<IExpression<Task<object?>>> expressionMock = new();
        expressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expressionResult));

        // Setting up MapExpression with its content
        Dictionary<string, IExpression<Task<object?>>> keysAndExpressions = new()
        {
            { expressionKey, expressionMock.Object },
        };
        MapExpression mapExpressionBlock = new(keysAndExpressions);

        Dictionary<string, object?> result = await mapExpressionBlock.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.ContainsKey(expressionKey));
        Assert.AreEqual(expressionResult, result[expressionKey]);
        expressionMock.Verify(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()), Times.Once());
        expressionMock.VerifyNoOtherCalls();
    }
}
