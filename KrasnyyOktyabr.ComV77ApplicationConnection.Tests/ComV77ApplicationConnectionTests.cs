using System.Runtime.Versioning;
using KrasnyyOktyabr.ComV77ApplicationConnection.Contracts.Configuration;

namespace KrasnyyOktyabr.ComV77ApplicationConnection.Tests;

[TestClass]
[SupportedOSPlatform("windows")]
public class ComV77ApplicationConnectionTests
{
    [TestMethod]
    public void PropertiesEquals_WhenEqual_ShouldReturnTrue()
    {
        string infobasePath = "TestPath";
        string username = "TestUsername";
        string password = "TestPassword";

        ConnectionProperties properties = new(infobasePath, username, password);
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

        ConnectionProperties properties1 = new(infobasePath1, username, password);
        ConnectionProperties properties1Copy = properties1 with { };
        ConnectionProperties properties2 = new(infobasePath2, username, password);

        Assert.IsTrue(properties.TryAdd(properties1, null));
        Assert.IsFalse(properties.TryAdd(properties1Copy, null));
        Assert.IsTrue(properties.TryAdd(properties2, null));
        Assert.AreEqual(2, properties.Count);
    }
}
