using Moq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ForeachExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ForeachExpression_WhenItemsExpressionNull_ShouldThrowArgumentNullException()
    {
        new ForeachExpression(null!, null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ForeachExpression_WhenInnerExpressionNull_ShouldThrowArgumentNullException()
    {
        Mock<IExpression<Task<object?[]>>> itemsExpressionMock = new();

        new ForeachExpression(itemsExpressionMock.Object, null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldRunLoop()
    {
        object?[] items = ["item1", "item2"];

        // Setting items expression
        Mock<IExpression<Task<object?[]>>> itemsExpressionMock = new();
        itemsExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Setting up inner expression
        Mock<IExpression<Task>> innerExpressionMock = new();
        innerExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Setting up foreach expression
        ForeachExpression foreachExpression = new(itemsExpressionMock.Object, innerExpressionMock.Object);

        Mock<IContext> contextMock = new();

        await foreachExpression.InterpretAsync(contextMock.Object);

        itemsExpressionMock.Verify(e => e.InterpretAsync(contextMock.Object, It.IsAny<CancellationToken>()), Times.Once);
        innerExpressionMock.Verify(e => e.InterpretAsync(contextMock.Object, It.IsAny<CancellationToken>()), Times.Exactly(items.Length));
        contextMock.Verify(c => c.UpdateForeachCursor(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<int>()), Times.Exactly(items.Length));
        contextMock.Verify(c => c.ClearForeachCursor(It.IsAny<string>()), Times.Once);
    }
}
