using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class AndExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AndExpression_WhenLeftExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<bool>>> rightExpressionMock = new();

        new AndExpression(null!, rightExpressionMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void AndExpression_WhenRightExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<bool>>> leftExpressionMock = new();

        new AndExpression(leftExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenTrueAndTrue_ShouldReturnTrue()
    {
        Assert.IsTrue(await RunAndExpression(true, true));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenTrueAndFalse_ShouldReturnFalse()
    {
        Assert.IsFalse(await RunAndExpression(true, false));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenFalseAndTrue_ShouldReturnFalse()
    {
        Assert.IsFalse(await RunAndExpression(false, true));
    }

    [TestMethod]
    public async Task InterpretAsync_WhenFalseAndFalse_ShouldReturnFalse()
    {
        Assert.IsFalse(await RunAndExpression(false, false));
    }

    private async Task<bool> RunAndExpression(bool leftExpressionResult, bool rightExpressionResult)
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

        AndExpression andExpression = new(leftExpressionMock.Object, rightExpressionMock.Object);

        return await andExpression.InterpretAsync(GetEmptyExpressionContext());
    }
}
