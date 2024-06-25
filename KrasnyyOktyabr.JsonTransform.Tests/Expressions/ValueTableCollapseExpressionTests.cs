using KrasnyyOktyabr.JsonTransform.Numerics;
using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableCollapseExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenValueTableNull_ShouldThrowInterpretException()
    {
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IValueTable)null!);

        Mock<IExpression<Task<string>>> columnsToGroupStringExpressionMock = new();
        columnsToGroupStringExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        Mock<IExpression<Task<string>>> columnsToSumStringExpressionMock = new();
        columnsToSumStringExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(string.Empty);

        ValueTableCollapseExpression valueTableCollapseExpression = new(valueTableExpressionMock.Object, columnsToSumStringExpressionMock.Object, columnsToSumStringExpressionMock.Object);

        await valueTableCollapseExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldSelectLine()
    {
        // Setting up value table
        string columnToGroup1 = "ColumnToGroup1";
        string columnToGroup2 = "ColumnToGroup2";
        string columnToCollapse1 = "ColumnToCollapse1";
        string columnToCollapse2 = "ColumnToCollapse2";
        Number term1 = new(2);
        Number term2 = new(3);
        Number term3 = new(4);
        ValueTable valueTable = new([columnToGroup1, columnToGroup2, columnToCollapse1, columnToCollapse2]);

        valueTable.AddLine();
        valueTable.SetValue(columnToGroup1, "TestGroup");
        valueTable.SetValue(columnToGroup2, "TestGroup");
        valueTable.SetValue(columnToCollapse1, term1);
        valueTable.SetValue(columnToCollapse2, term2);

        valueTable.AddLine();
        valueTable.SetValue(columnToGroup1, "TestGroup");
        valueTable.SetValue(columnToGroup2, "TestGroup");
        valueTable.SetValue(columnToCollapse1, term2);
        valueTable.SetValue(columnToCollapse2, term3);

        Assert.AreEqual(2, valueTable.Count);

        // Setting up value table expression mock
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueTable);

        // Setting up columns to group string expression mock
        Mock<IExpression<Task<string>>> columnsToGroupStringExpressionMock = new();
        columnsToGroupStringExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync($"{columnToGroup1}, {columnToGroup2}");

        // Setting up columns to group string expression mock
        Mock<IExpression<Task<string>>> columnsToSumStringExpressionMock = new();
        columnsToSumStringExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync($"{columnToCollapse1}, {columnToCollapse2}");

        ValueTableCollapseExpression valueTableCollapseExpression = new(valueTableExpressionMock.Object, columnsToGroupStringExpressionMock.Object, columnsToSumStringExpressionMock.Object);

        await valueTableCollapseExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(1, valueTable.Count);

        valueTable.SelectLine(0);

        Assert.AreEqual(term1 + term2, valueTable.GetValue(columnToCollapse1));
        Assert.AreEqual(term2 + term3, valueTable.GetValue(columnToCollapse2));
    }
}
