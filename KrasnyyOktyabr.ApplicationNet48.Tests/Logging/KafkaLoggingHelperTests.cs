﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrasnyyOktyabr.ApplicationNet48.Logging.Tests;

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

        string expected = "Te ... age";

        string actual = KafkaLoggingHelper.ShortenMessage(message, message.Length - 1);

        Assert.AreEqual(expected, actual);
    }
}
