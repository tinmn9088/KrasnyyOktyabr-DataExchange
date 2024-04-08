namespace KrasnyyOktyabr.ComV77ApplicationConnection.Tests;

[TestClass]
public class ComV77ApplicationConnectionTests
{
    [TestMethod]
    public void PropertiesEquals_WhenEqual_ShouldReturnTrue()
    {
        string infobasePath = "TestPath";
        string username = "TestUsername";
        string password = "TestPassword";

        IComV77ApplicationConnection.Properties properties = new(infobasePath, username, password);
        IComV77ApplicationConnection.Properties propertiesCopy = properties with { };

        Assert.AreNotSame(properties, propertiesCopy);
        Assert.AreEqual(properties, propertiesCopy);
    }

    [TestMethod]
    public void Properties_AsDictionaryKeys()
    {
        string infobasePath1 = "TestPath1";
        string infobasePath2 = "TestPath2";
        string username = "TestUsername";
        string password = "TestPassword";

        Dictionary<IComV77ApplicationConnection.Properties, object?> properties = [];

        IComV77ApplicationConnection.Properties properties1 = new(infobasePath1, username, password);
        IComV77ApplicationConnection.Properties properties1Copy = properties1 with { };
        IComV77ApplicationConnection.Properties properties2 = new(infobasePath2, username, password);

        Assert.IsTrue(properties.TryAdd(properties1, null));
        Assert.IsFalse(properties.TryAdd(properties1Copy, null));
        Assert.IsTrue(properties.TryAdd(properties2, null));
        Assert.AreEqual(2, properties.Count);
    }
}
