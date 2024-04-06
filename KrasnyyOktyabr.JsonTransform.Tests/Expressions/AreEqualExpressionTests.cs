using Moq;
using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class AreEqualExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AreEqualExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<object?>>> rightExpressionMock = new();

        new AreEqualExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AreEqualExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<object?>>> leftExpressionMock = new();

        new AreEqualExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenSameObjects_ShouldReturnTrue()
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

        await TestEquality(leftExpressionMock.Object, rightExpressionMock.Object);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenBothNull_ShouldReturnTrue()
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

        await TestEquality(leftExpressionMock.Object, rightExpressionMock.Object);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenEqualValues_ShouldReturnTrue()
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

        await TestEquality(leftExpressionMock.Object, rightExpressionMock.Object);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenNonEqualValues_ShouldReturnFalse()
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

        await TestInequality(leftExpressionMock.Object, rightExpressionMock.Object);
    }

    private static async Task TestEquality(IExpression<Task<object?>> x, IExpression<Task<object?>> y)
    {
        // reflexivity
        Assert.IsTrue(await Run(x, x));
        Assert.IsTrue(await Run(y, y));

        // symmetry
        Assert.IsTrue(await Run(x, y));
        Assert.IsTrue(await Run(y, x));
    }

    private static async Task TestInequality(IExpression<Task<object?>> x, IExpression<Task<object?>> y)
    {
        // reflexivity
        Assert.IsTrue(await Run(x, x));
        Assert.IsTrue(await Run(y, y));

        // symmetry
        Assert.IsFalse(await Run(x, y));
        Assert.IsFalse(await Run(y, x));
    }

    private static async Task<bool> Run(IExpression<Task<object?>> leftExpression, IExpression<Task<object?>> rightExpression)
    {
        return await new AreEqualExpression(leftExpression, rightExpression).InterpretAsync(GetEmptyExpressionContext());
    }
}
