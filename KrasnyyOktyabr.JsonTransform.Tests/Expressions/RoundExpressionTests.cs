using KrasnyyOktyabr.JsonTransform.Numerics;
using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class RoundExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RoundExpression_WhenValueExpressionNull_ShouldThrowArgumentNullException()
    {
        new RoundExpression(null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnRounded()
    {
        Number valueExpressionResult = new(2.4);

        // Setting up value expression
        Mock<IExpression<Task<Number>>> valueExpressionMock = new();
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(valueExpressionResult));

        RoundExpression roundExpression = new(valueExpressionMock.Object);

        Number expected = new(2);

        Number actual = await roundExpression.InterpretAsync(GetEmptyExpressionContext(), default);

        Assert.AreEqual(expected, actual);
    }
}
