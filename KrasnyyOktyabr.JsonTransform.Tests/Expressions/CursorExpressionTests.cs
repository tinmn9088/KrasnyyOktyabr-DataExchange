using Moq;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class CursorExpressionTests
{
    [TestMethod]
    public void CursorExpression_WhenNameExpressionNull_ShouldCreateCursorExpression()
    {
        new CursorExpression(null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnCursor()
    {
        object? cursorValue = "TestValue";

        // Setting up foreach cursor expression
        CursorExpression cursorExpression = new();

        Mock<IContext> contextMock = new();
        contextMock
            .Setup(c => c.GetCursor())
            .Returns(cursorValue);

        object? actual = await cursorExpression.InterpretAsync(contextMock.Object);

        contextMock.Verify(c => c.GetCursor(), Times.Once);
    }

    [TestMethod]
    public async Task InterpretAsync_WhenNameSpecified_ShouldReturnCursor()
    {
        string cursorName = "TestName";
        object? cursorValue = "TestValue";

        // Setting name expression
        Mock<IExpression<Task<string>>> nameExpressionMock = new();
        nameExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorName);

        // Setting up cursor expression
        CursorExpression cursorExpression = new(nameExpressionMock.Object);

        Mock<IContext> contextMock = new();
        contextMock
            .Setup(c => c.GetCursor(It.Is<string>(n => n == cursorName)))
            .Returns(cursorValue);

        object? actual = await cursorExpression.InterpretAsync(contextMock.Object);

        nameExpressionMock.Verify(e => e.InterpretAsync(contextMock.Object, It.IsAny<CancellationToken>()), Times.Once);
        contextMock.Verify(c => c.GetCursor(It.Is<string>(n => n == cursorName)), Times.Once);
    }
}
