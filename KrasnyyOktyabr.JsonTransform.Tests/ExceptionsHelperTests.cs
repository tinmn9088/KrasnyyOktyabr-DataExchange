namespace KrasnyyOktyabr.JsonTransform.Tests;

[TestClass]
public class ExceptionsHelperTests
{
    [TestMethod]
    public void BuildTypeNameWithParameters_ShouldReturnTypeNameWithParameters()
    {
        Type type = typeof(Action<Task<string>, Task<int>, Task<Action<string>>>);
        string expected = "Action<Task<String>, Task<Int32>, Task<Action<String>>>";

        string actual = ExceptionsHelper.BuildTypeNameWithParameters(type);

        Assert.AreEqual(expected, actual);
    }
}
