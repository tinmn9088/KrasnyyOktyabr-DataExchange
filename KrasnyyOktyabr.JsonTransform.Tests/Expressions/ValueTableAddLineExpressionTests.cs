using KrasnyyOktyabr.JsonTransform.Structures;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ValueTableAddLineExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(InterpretException))]
    public async Task InterpretAsync_WhenValueTableNull_ShouldThrowInterpretException()
    {
        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IValueTable)null!);

        ValueTableAddLineExpression valueTableAddLineExpression = new(valueTableExpressionMock.Object);

        await valueTableAddLineExpression.InterpretAsync(CreateEmptyExpressionContext());
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldAddLine()
    {
        ValueTable valueTable = new(["TestColumn"]);

        Mock<IExpression<Task<IValueTable>>> valueTableExpressionMock = new();
        valueTableExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(valueTable);

        ValueTableAddLineExpression valueTableAddLineExpression = new(valueTableExpressionMock.Object);

        await valueTableAddLineExpression.InterpretAsync(CreateEmptyExpressionContext());

        Assert.AreEqual(1, valueTable.Count);
    }
}
