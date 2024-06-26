using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableCountExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenValueTableNull_ShouldThrowInterpretException()
    {
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IValueTable)null!);

        ValueTableCountExpression valueTableCountExpression = new(valueTableExpressionMock.Object);

        await valueTableCountExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnLinesCount()
    {
        // Setting up value table
        string columnName = "TestColumn";
        ValueTable valueTable = new([columnName]);

        valueTable.AddLine();
        valueTable.AddLine();

        Assert.AreEqual(2, valueTable.Count);

        // Setting up value table expression mock
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueTable);

        ValueTableCountExpression valueTableGetValueExpression = new(valueTableExpressionMock.Object);

        object? count = await valueTableGetValueExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(2L, count);
    }
}
