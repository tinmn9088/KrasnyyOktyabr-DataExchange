using KrasnyyOktyabr.ComV77Application.Contracts.Configuration;

namespace KrasnyyOktyabr.ComV77Application.Tests;

[TestClass]
public class ComV77ApplicationConnectionTests
{
    [TestMethod]
    public void PropertiesEquals_WhenEqual_ShouldReturnTrue()
    {
        string infobasePath = "TestPath";
        string username = "TestUsername";
        string password = "TestPassword";

        ConnectionProperties properties = new(
            infobasePath: infobasePath,
            username: username,
            password: password
        );
        ConnectionProperties propertiesCopy = properties with { };

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

        Dictionary<ConnectionProperties, object?> properties = [];

        ConnectionProperties properties1 = new(
            infobasePath: infobasePath1,
            username: username,
            password: password
        );
        ConnectionProperties properties1Copy = properties1 with { };
        ConnectionProperties properties2 = new(
            infobasePath: infobasePath2,
            username: username,
            password: password
        );

        properties.Add(properties1, null);

        Assert.ThrowsException<ArgumentException>(() => properties.Add(properties1Copy, null));

        properties.Add(properties2, null);

        Assert.IsTrue(properties.ContainsKey(properties1));
        Assert.IsTrue(properties.ContainsKey(properties1Copy));
        Assert.IsTrue(properties.ContainsKey(properties2));
        Assert.AreEqual(2, properties.Count);
    }
}
