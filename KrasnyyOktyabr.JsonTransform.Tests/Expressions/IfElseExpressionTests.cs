using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class IfElseExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IfElseExpression_WhenConditionExpressionNull_ShouldThrowArgumentNullException()
    {
        new IfElseExpression(null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IfElseExpression_WhenThenExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();

        new IfElseExpression(conditionExpressionMock.Object, null!);
    }

    [TestMethod]
    public void IfElseExpression_WhenElseExpressionNull_ShouldCreateIfElseExpression()
    {
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        Mock<IExpression<Task>> thenExpressionMock = new();

        new IfElseExpression(conditionExpressionMock.Object, thenExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenConditionTrue_ShouldRunThen()
    {
        // Setting condition expression
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        conditionExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));

        // Setting up then expression
        Mock<IExpression<Task>> thenExpressionMock = new();
        thenExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Setting up while expression
        IfElseExpression ifElseExpression = new(conditionExpressionMock.Object, thenExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        await ifElseExpression.InterpretAsync(context);

        conditionExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once);
        thenExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenConditionFalse_ShouldRunNotThen()
    {
        // Setting condition expression
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        conditionExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(false));

        // Setting up then expression
        Mock<IExpression<Task>> thenExpressionMock = new();
        thenExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Setting up while expression
        IfElseExpression ifElseExpression = new(conditionExpressionMock.Object, thenExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        await ifElseExpression.InterpretAsync(context);

        conditionExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once);
        thenExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenConditionFalseAndElseExpressionPresent_ShouldRunElse()
    {
        // Setting condition expression
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        conditionExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(false));

        // Setting up then expression
        Mock<IExpression<Task>> thenExpressionMock = new();
        thenExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Setting up else expression
        Mock<IExpression<Task>> elseExpressionMock = new();
        elseExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Setting up while expression
        IfElseExpression ifElseExpression = new(conditionExpressionMock.Object, thenExpressionMock.Object, elseExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        await ifElseExpression.InterpretAsync(context);

        conditionExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once);
        thenExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Never);
        elseExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Once);
    }
}
