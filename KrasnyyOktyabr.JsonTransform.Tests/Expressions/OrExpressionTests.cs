using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OrExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<bool>>> rightExpressionMock = new();

        new OrExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OrExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<bool>>> leftExpressionMock = new();

        new OrExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenTrueAndTrue_ShouldReturnTrue()
    {
        Assert.IsTrue(await RunOrExpression(true, true));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenTrueAndFalse_ShouldReturnTrue()
    {
        Assert.IsTrue(await RunOrExpression(true, false));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenFalseAndTrue_ShouldReturnTrue()
    {
        Assert.IsTrue(await RunOrExpression(false, true));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenFalseAndFalse_ShouldReturnFalse()
    {
        Assert.IsFalse(await RunOrExpression(false, false));
    }

    private async Task<bool> RunOrExpression(bool leftExpressionResult, bool rightExpressionResult)
    {
        // Setting up left expression
        Mock<IExpression<Task<bool>>> leftExpressionMock = new();
        leftExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(leftExpressionResult));

        // Setting up right expression
        Mock<IExpression<Task<bool>>> rightExpressionMock = new();
        rightExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(rightExpressionResult));

        OrExpression orExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        return await orExpression.InterpretAsync(GetEmptyExpressionContext());
    }
}
