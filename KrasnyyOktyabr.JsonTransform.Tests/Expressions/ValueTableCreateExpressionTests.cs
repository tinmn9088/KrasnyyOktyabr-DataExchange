using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableCreateExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CreateValueTableExpression_WhenColumnsStringExpressionNull_ShouldThrowArgumentNullException()
    {
        new ValueTableCreateExpression(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenColumnsStringNull_ShouldThrowInterpretException()
    {
        Mock<IExpression<Task<string>>> columnsStringExpressionMock = new();
        columnsStringExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);

        ValueTableCreateExpression valueTableCreateExpression = new(columnsStringExpressionMock.Object);

        await valueTableCreateExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldCreateValueTable()
    {
        Mock<IExpression<Task<string>>> columnsStringExpressionMock = new();
        columnsStringExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("Column1, Column2, Column3");

        ValueTableCreateExpression valueTableCreateExpression = new(columnsStringExpressionMock.Object);

        IValueTable table = await valueTableCreateExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.IsNotNull(table);
    }
}
