using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class AreNotEqualExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AreNotEqualExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<object?>>> rightExpressionMock = new();

        new AreNotEqualExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AreNotEqualExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<object?>>> leftExpressionMock = new();

        new AreNotEqualExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenSameObjects_ShouldReturnFalse()
    {
        string expressionsResult = "TestValue";

        // Setting up left expression
        Mock<IExpression<Task<object?>>> leftExpressionMock = new();
        leftExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expressionsResult));

        // Setting up right expression
        Mock<IExpression<Task<object?>>> rightExpressionMock = new();
        rightExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)expressionsResult));

        await TestReflexivity(leftExpressionMock.Object, rightExpressionMock.Object);

        Assert.IsFalse(await Run(leftExpressionMock.Object, rightExpressionMock.Object));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenBothNull_ShouldReturnFalse()
    {
        // Setting up left expression
        Mock<IExpression<Task<object?>>> leftExpressionMock = new();
        leftExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)null));

        // Setting up right expression
        Mock<IExpression<Task<object?>>> rightExpressionMock = new();
        rightExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)null));

        await TestReflexivity(leftExpressionMock.Object, rightExpressionMock.Object);

        Assert.IsFalse(await Run(leftExpressionMock.Object, rightExpressionMock.Object));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenEqualValues_ShouldReturnFalse()
    {
        JObject leftExpressionResult = new()
        {
            { "Key", "Value" },
        };
        JObject rightExpressionResult = new()
        {
            { "Key", "Value" },
        };

        // Setting up left expression
        Mock<IExpression<Task<object?>>> leftExpressionMock = new();
        leftExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)leftExpressionResult));

        // Setting up right expression
        Mock<IExpression<Task<object?>>> rightExpressionMock = new();
        rightExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)rightExpressionResult));

        await TestReflexivity(leftExpressionMock.Object, rightExpressionMock.Object);

        Assert.IsFalse(await Run(leftExpressionMock.Object, rightExpressionMock.Object));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenNonEqualValues_ShouldReturnTrue()
    {
        int leftExpressionResult = 66;
        int rightExpressionResult = 99;

        // Setting up left expression
        Mock<IExpression<Task<object?>>> leftExpressionMock = new();
        leftExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)leftExpressionResult));

        // Setting up right expression
        Mock<IExpression<Task<object?>>> rightExpressionMock = new();
        rightExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((object?)rightExpressionResult));

        await TestReflexivity(leftExpressionMock.Object, rightExpressionMock.Object);

        Assert.IsTrue(await Run(leftExpressionMock.Object, rightExpressionMock.Object));
    }

    private static async Task TestReflexivity(IExpression<Task<object?>> x, IExpression<Task<object?>> y)
    {
        Assert.IsFalse(await Run(x, x));
        Assert.IsFalse(await Run(y, y));
    }

    private static async Task<bool> Run(IExpression<Task<object?>> leftExpression, IExpression<Task<object?>> rightExpression)
    {
        return await new AreNotEqualExpression(leftExpression, rightExpression).InterpretAsync(CreateEmptyExpressionContext());
    }
}
