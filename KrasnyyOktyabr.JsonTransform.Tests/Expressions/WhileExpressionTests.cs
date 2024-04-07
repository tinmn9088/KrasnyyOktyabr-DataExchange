using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class WhileExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void WhileExpression_WhenConditionExpressionNull_ShouldThrowArgumentNullException()
    {
        new WhileExpression(null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void WhileExpression_WhenInnerExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();

        new WhileExpression(conditionExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldRunLoop()
    {
        int count = 0;
        int limit = 3;

        // Setting condition expression
        Mock<IExpression<Task<bool>>> conditionExpressionMock = new();
        conditionExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => count < limit);

        // Setting up inner expression
        Mock<IExpression<Task>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => count++);

        // Setting up while expression
        WhileExpression whileExpression = new(conditionExpressionMock.Object, innerExpressionMock.Object);

        Context context = CreateEmptyExpressionContext();

        await whileExpression.InterpretAsync(context);

        conditionExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Exactly(limit + 1));
        innerExpressionMock.Verify(e => e.InterpretAsync(context, It.IsAny<CancellationToken>()), Times.Exactly(limit));
    }
}
