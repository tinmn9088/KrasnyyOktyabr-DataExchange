using System.Collections.Generic;
using KrasnyyOktyabr.ApplicationNet48.Modules.Kafka.CoreServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ConfigurationBuilder = Microsoft.Extensions.Configuration.ConfigurationBuilder;

namespace KrasnyyOktyabr.ApplicationNet48.Tests.Services.Kafka;

[TestClass]
public class KafkaServiceTests
{
    private static Mock<ILogger<KafkaService>>? s_loggerMock;

    private static KafkaService? s_kafkaService;

    private static Dictionary<string, string?> TestKafkaSettings => new()
    {
        { "Kafka:Socket", "TestSocketValue" }
    };

    [TestInitialize]
    public void TestInitialize()
    {
        s_loggerMock = new();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(TestKafkaSettings)
            .Build();

        s_kafkaService = new(configuration, s_loggerMock.Object);
    }

    [TestMethod]
    public void BuildTopicName_ShouldBuildTopicName()
    {
        // Depends on TransliterationHelper
        Assert.AreEqual("name1_name2", s_kafkaService!.BuildTopicName("name1", "name2"));
    }
}
