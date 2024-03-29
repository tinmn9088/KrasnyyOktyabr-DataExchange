namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class ObjectCastExpressionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ObjectCastExpression_WhenInnerNull_ShouldThrowArgumentNullException()
    {
        new ObjectCastExpression(null!);
    }
}
