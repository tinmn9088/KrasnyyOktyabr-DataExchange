using Microsoft.Extensions.Configuration;
using Moq;

namespace KrasnyyOktyabr.Application.Services.Kafka.Tests;

[TestClass]
public class KafkaServiceTests
{
    private static Mock<ITransliterationService>? s_transliterationServiceMock;

    private static KafkaService? s_kafkaService;

    private static Dictionary<string, string?> TestKafkaSettings => new()
    {
        { "Kafka:Socket", "TestSocketValue" }
    };

    [TestInitialize]
    public void TestInitialize()
    {
        s_transliterationServiceMock = new();

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(TestKafkaSettings)
            .Build();

        s_kafkaService = new(configuration, s_transliterationServiceMock.Object);
    }

    [TestMethod]
    public void BuildTopicName_ShouldBuildTopicName()
    {
        s_transliterationServiceMock
            !.Setup(s => s.TransliterateToLatin(It.IsAny<string>()))
            .Returns<string>(source => source);

        Assert.AreEqual("name1_name2", s_kafkaService!.BuildTopicName("name1", "name2"));
    }
}
