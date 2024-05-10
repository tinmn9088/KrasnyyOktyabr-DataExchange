using KrasnyyOktyabr.JsonTransform.Expressions.DataResolve;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class DataResolveExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DataResolveExpression_WhenResolverExpressionNull_ShouldThrowArgumentNullException()
    {
        new DataResolveExpression(null!, null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DataResolveExpression_WhenArgsExpressionsNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<string>>> resolverExpressionMock = new();

        new DataResolveExpression(resolverExpressionMock.Object, null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DataResolveExpression_WhenDataResolveServiceNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<string>>> resolverExpressionMock = new();
        Mock<IExpression<Task<Dictionary<string, object?>>>> argsExpressionMock = new();

        new DataResolveExpression(resolverExpressionMock.Object, argsExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldFormatString()
    {
        // Setting up resolver expression
        Mock<IExpression<Task<string>>> nullResolverExpressionMock = new();
        string resolverName = "TestResolverName";
        nullResolverExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resolverName);

        // Setting up args expression
        Mock<IExpression<Task<Dictionary<string, object?>>>> argsExpressionMock = new();
        argsExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Setting up data resolve service mock
        string expected = "TestValue";
        Mock<IDataResolveService> dataResolveServiceMock = new();
        dataResolveServiceMock
            .Setup(s => s.ResolveAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object?>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        // Setting up string format expression
        DataResolveExpression dataResolveExpression = new(nullResolverExpressionMock.Object, argsExpressionMock.Object, dataResolveServiceMock.Object);

        object? actual = await dataResolveExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
        dataResolveServiceMock.Verify(s => s.ResolveAsync(It.Is<string>(s => s == resolverName), It.IsAny<Dictionary<string, object?>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenResolverExpressionResultNull_ShouldThrowInterpretException()
    {
        // Setting up null resolver expression
        Mock<IExpression<Task<string>>> nullResolverExpressionMock = new();
        string resolverName = null!;
        nullResolverExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resolverName);

        // Setting up args expression
        Mock<IExpression<Task<Dictionary<string, object?>>>> argsExpressionMock = new();
        argsExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Setting up data resolve service mock
        Mock<IDataResolveService> dataResolveServiceMock = new();

        // Setting up string format expression
        DataResolveExpression dataResolveExpression = new(nullResolverExpressionMock.Object, argsExpressionMock.Object, dataResolveServiceMock.Object);

        await dataResolveExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenFormatExpressionResultNull_ShouldThrowInterpretException()
    {
        // Setting up resolver expression
        Mock<IExpression<Task<string>>> nullResolverExpressionMock = new();
        string resolverName = "TestResolverName";
        nullResolverExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(resolverName);

        // Setting up null args expression
        Mock<IExpression<Task<Dictionary<string, object?>>>> argsExpressionMock = new();
        argsExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Dictionary<string, object?>)null!);

        // Setting up data resolve service mock
        Mock<IDataResolveService> dataResolveServiceMock = new();

        // Setting up string format expression
        DataResolveExpression dataResolveExpression = new(nullResolverExpressionMock.Object, argsExpressionMock.Object, dataResolveServiceMock.Object);

        await dataResolveExpression.InterpretAsync(CreateEmptyExpressionContext());
    }
}
