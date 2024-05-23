using Moq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class CursorIndexExpressionTests
{
    [TestMethod]
    public void CursorIndexExpression_WhenNameExpressionNull_ShouldCreateCursorIndexExpression()
    {
        new CursorIndexExpression(null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnCursor()
    {
        int cursorIndex = 2;

        // Setting up cursor index expression
        CursorIndexExpression cursorIndexExpression = new();

        Mock<IContext> contextMock = new();
        contextMock
            .Setup(c => c.GetCursorIndex())
            .Returns(cursorIndex);

        long actual = await cursorIndexExpression.InterpretAsync(contextMock.Object);

        contextMock.Verify(c => c.GetCursorIndex(), Times.Once);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenNameSpecified_ShouldReturnCursor()
    {
        string cursorName = "TestName";
        int cursorIndex = 2;

        // Setting name expression
        Mock<IExpression<Task<string>>> nameExpressionMock = new();
        nameExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorName);

        // Setting up cursor index expression
        CursorIndexExpression cursorIndexExpression = new(nameExpressionMock.Object);

        Mock<IContext> contextMock = new();
        contextMock
            .Setup(c => c.GetCursorIndex(It.Is<string>(n => n == cursorName)))
            .Returns(cursorIndex);

        long actual = await cursorIndexExpression.InterpretAsync(contextMock.Object);

        nameExpressionMock.Verify(e => e.InterpretAsync(contextMock.Object, It.IsAny<CancellationToken>()), Times.Once);
        contextMock.Verify(c => c.GetCursorIndex(It.Is<string>(n => n == cursorName)), Times.Once);
    }
}
