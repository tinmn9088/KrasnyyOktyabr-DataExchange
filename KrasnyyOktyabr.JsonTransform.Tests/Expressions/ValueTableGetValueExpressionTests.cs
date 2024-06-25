using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableGetValueExpressionTests
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

        ValueTableGetValueExpression valueTableGetValueExpression = new(valueTableExpressionMock.Object, columnExpressionMock.Object);

        await valueTableGetValueExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldSelectLine()
    {
        // Setting up value table
        string columnName = "TestColumn";
        string expected = "TestValue";
        ValueTable valueTable = new([columnName]);

        valueTable.AddLine();
        valueTable.SetValue(columnName, expected);

        Assert.AreEqual(1, valueTable.Count);

        // Setting up value table expression mock
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueTable);

        // Setting up column expression mock
        Mock<IExpression<Task<string>>> columnExpressionMock = new();
        columnExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(columnName);

        ValueTableGetValueExpression valueTableGetValueExpression = new(valueTableExpressionMock.Object, columnExpressionMock.Object);

        object? actual = await valueTableGetValueExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expected, actual);
    }
}
