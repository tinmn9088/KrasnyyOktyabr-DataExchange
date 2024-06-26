using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableAddColumnExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenValueTableNull_ShouldThrowInterpretException()
    {
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IValueTable)null!);

        Mock<IExpression<Task<string>>> columnExpressionMock = new();
        columnExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        ValueTableAddColumnExpression valueTableAddColumnExpression = new(valueTableExpressionMock.Object, columnExpressionMock.Object);

        await valueTableAddColumnExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldAddColumn()
    {
        // Setting up value table
        string column = "TestColumn";
        string columnToAdd = "NewColumn";
        ValueTable valueTable = new([column]);

        // Setting up value table expression mock
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueTable);

        // Setting up column expression mock
        Mock<IExpression<Task<string>>> columnExpressionMock = new();
        columnExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(columnToAdd);

        ValueTableAddColumnExpression valueTableAddColumnExpression = new(valueTableExpressionMock.Object, columnExpressionMock.Object);

        await valueTableAddColumnExpression.InterpretAsync(CreateEmptyExpressionContext());

        IReadOnlyList<string> columns = valueTable.Columns;

        Assert.AreEqual(2, columns.Count);
        Assert.AreEqual(columnToAdd, columns[1]);
    }
}
