namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

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
