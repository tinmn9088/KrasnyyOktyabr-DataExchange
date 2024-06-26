using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableSetValueExpressionTests
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

        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(null!);

        ValueTableSetValueExpression valueTableSetValueExpression = new(valueTableExpressionMock.Object, columnExpressionMock.Object, valueExpressionMock.Object);

        await valueTableSetValueExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldSetValue()
    {
        // Setting up value table
        string columnName = "TestColumn";
        string expectedValue = "TestValue";
        ValueTable valueTable = new([columnName]);

        valueTable.AddLine();

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

        // Setting up value expression mock
        Mock<IExpression<Task<object?>>> valueExpressionMock = new();
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedValue);

        ValueTableSetValueExpression valueTableSetValueExpression = new(valueTableExpressionMock.Object, columnExpressionMock.Object, valueExpressionMock.Object);

        await valueTableSetValueExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(expectedValue, valueTable.GetValue(columnName));
    }
}
