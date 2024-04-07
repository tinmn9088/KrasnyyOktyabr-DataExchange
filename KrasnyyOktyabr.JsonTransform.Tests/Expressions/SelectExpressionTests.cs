using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class SelectExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void SelectExpression_WhenPathExpressionNull_ShouldThrowArgumentNullException()
    {
        new SelectExpression(null!);
    }

    [TestMethod]
    public void SelectExpression_WhenIsOptionalExpressionNull_ShouldCreateSelectExpression()
    {
        Mock<IExpression<Task<string>>> pathExpressionMock = new();

        new SelectExpression(pathExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldSelect()
    {
        // Setting up path expression
        Mock<IExpression<Task<string>>> pathExpressionMock = new();
        string pathString = "nestedObject.key";
        pathExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(pathString));

        // Setting up select expression
        SelectExpression selectExpression = new(pathExpressionMock.Object);

        JToken expected = "TestValue";

        JObject input = new()
        {
            {
                "nestedObject",
                new JObject()
                {
                    { "key", expected },
                }
            }
        };
        Context context = new(input);

        JToken? actual = await selectExpression.InterpretAsync(context);

        pathExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [ExpectedException(typeof(SelectExpression.PathReturnedNothingException))]
    public async Task InterpretAsync_WhenPathInvalid_ShouldThrowPathReturnedNothingException()
    {
        // Setting up path expression
        Mock<IExpression<Task<string>>> pathExpressionMock = new();
        string pathString = "invalidPath";
        pathExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(pathString));

        // Setting up select expression
        SelectExpression selectExpression = new(pathExpressionMock.Object);

        await selectExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_WhenPathInvalidAndIsOptionalExpressionReturnsTrue_ShouldReturnNull()
    {
        // Setting up path expression
        Mock<IExpression<Task<string>>> pathExpressionMock = new();
        string pathString = "invalidPath";
        pathExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(pathString));

        // Setting up is optional expression
        Mock<IExpression<Task<bool>>> isOptionalExpressionMock = new();
        isOptionalExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));

        // Setting up select expression
        SelectExpression selectExpression = new(pathExpressionMock.Object, isOptionalExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        JToken? actual = await selectExpression.InterpretAsync(context);

        pathExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once());
        Assert.IsNull(actual);
    }
}
