using KrasnyyOktyabr.ApplicationNet48.Common.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrasnyyOktyabr.ApplicationNet48.Tests.Logging;

[TestClass]
public class MessageHelperTests
{
    [TestMethod]
    public void ShortenMessage_WhenMessageShorterThanLimit_ShoudldReturnMessage()
    {
        string message = "TestMessage";

        Assert.AreEqual(message, MessageHelper.ShortenMessage(message, message.Length + 1));
    }

    [TestMethod]
    public void ShortenMessage_ShoudldShortenMessage()
    {
        string message = "TestMessage";

        string expected = "Te ... age";

        string actual = MessageHelper.ShortenMessage(message, message.Length - 1);

        Assert.AreEqual(expected, actual);
    }
}
