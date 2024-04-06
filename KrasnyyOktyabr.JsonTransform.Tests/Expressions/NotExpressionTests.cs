using Moq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class NotExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void NotExpression_WhenValueExpressionNull_ShouldThrowArgumentNullException()
    {
        new NotExpression(null!);
    }

    [TestMethod]
    public async Task InterpretAsync_ShouldReturnRounded()
    {
        bool valueExpressionResult = true;

        // Setting up value expression
        Mock<IExpression<Task<bool>>> valueExpressionMock = new();
        valueExpressionMock
            .Setup(e => e.InterpretAsync(It.IsAny<IContext>(), It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(valueExpressionResult));

        NotExpression notExpression = new(valueExpressionMock.Object);

        bool result = await notExpression.InterpretAsync(GetEmptyExpressionContext());

        Assert.IsFalse(result);
    }
}
