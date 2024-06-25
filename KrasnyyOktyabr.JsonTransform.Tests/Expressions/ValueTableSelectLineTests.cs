using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableSelectLineTests
{
    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenValueTableNull_ShouldThrowInterpretException()
    {
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IValueTable)null!);

        Mock<IExpression<Task<long>>> indexExpressionMock = new();
        indexExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        ValueTableSelectLineExpression valueTableSelectLineExpression = new(valueTableExpressionMock.Object, indexExpressionMock.Object);

        await valueTableSelectLineExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldSelectLine()
    {
        // Setting up value table
        string columnName = "TestColumn";
        string expectedValue = "TestValue";
        ValueTable valueTable = new([columnName]);

        valueTable.AddLine();
        valueTable.AddLine();
        valueTable.SetValue(columnName, expectedValue);
        valueTable.SelectLine(0);

        Assert.AreEqual(2, valueTable.Count);

        // Setting up value table expression mock
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueTable);

        // Setting up index expression mock
        Mock<IExpression<Task<long>>> indexExpressionMock = new();
        indexExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        ValueTableSelectLineExpression valueTableSelectLineExpression = new(valueTableExpressionMock.Object, indexExpressionMock.Object);

        await valueTableSelectLineExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expectedValue, valueTable.GetValue(columnName));
    }
}
