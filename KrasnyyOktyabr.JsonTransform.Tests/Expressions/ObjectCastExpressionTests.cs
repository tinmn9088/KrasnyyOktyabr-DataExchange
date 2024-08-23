using KrasnyyOktyabr.JsonTransform.Expressions;

namespace KrasnyyOktyabr.JsonTransform.Tests.Expressions;

[TestClass]
public class ObjectCastExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ObjectCastExpression_WhenInnerExpressionNull_ShouldThrowArgumentNullException()
    {
        new ObjectCastExpression(null!);
    }
}
