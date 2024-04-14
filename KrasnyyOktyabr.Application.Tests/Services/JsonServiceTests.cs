namespace KrasnyyOktyabr.Application.Services.Tests;

[TestClass]
public class JsonServiceTests
{
    private static readonly JsonService s_jsonService = new JsonService();

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RemoveEmptyPropertiesAndAdd_WhenJsonObjectNull_ShouldThrowArgumentNullException()
    {
        s_jsonService.RemoveEmptyPropertiesAndAdd(null!, []);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RemoveEmptyPropertiesAndAdd_WhenPropertiesToAddNull_ShouldThrowArgumentNullException()
    {
        s_jsonService.RemoveEmptyPropertiesAndAdd("{}", null!);
    }

    [TestMethod]
    public void RemoveEmptyPropertiesAndAdd_ShouldRemoveEmptyPropertiesAndAdd()
    {
        string json = "{\"Property1\":\"Value1\",\"EmptyProperty\":\"\",\"NullProperty\":null}";

        Dictionary<string, object?> propertiesToAdd = new()
        {
            { "Property2", "Value2" },
            { "Property3", null },
            { "Property4", 666 },
        };

        string expected = "{\"Property1\":\"Value1\",\"Property2\":\"Value2\",\"Property3\":null,\"Property4\":666}";

        string actual = s_jsonService.RemoveEmptyPropertiesAndAdd(json, propertiesToAdd);

        Assert.AreEqual(expected, actual);
    }
}
