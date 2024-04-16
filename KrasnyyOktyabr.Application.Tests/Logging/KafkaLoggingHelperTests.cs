namespace KrasnyyOktyabr.Application.Logging.Tests;

[TestClass]
public class KafkaLoggingHelperTests
{
    [TestMethod]
    public void ShortenMessage_WhenMessageShorterThanLimit_ShoudldReturnMessage()
    {
        string message = "TestMessage";

        Assert.AreEqual(message, KafkaLoggingHelper.ShortenMessage(message, message.Length + 1));
    }

    [TestMethod]
    public void ShortenMessage_ShoudldShortenMessage()
    {
        string message = "TestMessage";

        string expected = "Tes ... ge";

        string actual = KafkaLoggingHelper.ShortenMessage(message, message.Length - 1);

        Assert.AreEqual(expected, actual);
    }
}
